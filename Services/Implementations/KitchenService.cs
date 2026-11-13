using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.DTOs.Kitchens;
using Services.Exceptions;
using Services.Interfaces;

namespace Services.Implementations
{
    public class KitchenService : IKitchenService
    {
        private readonly IKitchenRepository _kitchenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public KitchenService(
            IKitchenRepository kitchenRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _kitchenRepository = kitchenRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách tất cả Kitchen.
        /// </summary>
        public async Task<List<KitchenResponse>> GetAllAsync()
        {
            // Repository lấy danh sách Kitchen
            var kitchens = await _kitchenRepository.GetAllAsync();

            // Chuyển Entity sang DTO
            return _mapper.Map<List<KitchenResponse>>(kitchens);
        }

        /// <summary>
        /// Lấy thông tin một Kitchen theo ID.
        /// </summary>
        public async Task<KitchenResponse> GetByIdAsync(int kitchenId)
        {
            // Tìm Kitchen theo khóa chính
            var kitchen = await _kitchenRepository
                .GetByIdAsync(kitchenId);

            if (kitchen == null)
            {
                throw new NotFoundException(
                    "Kitchen does not exist.");
            }

            return _mapper.Map<KitchenResponse>(kitchen);
        }

        /// <summary>
        /// Tạo Kitchen mới (Bếp tổng hoặc chi nhánh).
        /// Chỉ SystemAdmin được tạo (kiểm tra ở Controller).
        /// </summary>
        public async Task<KitchenResponse> CreateAsync(
            CreateKitchenRequest request)
        {
            // Chuẩn hóa tên trước khi lưu
            var kitchenName = request.KitchenName.Trim();

            // Kiểm tra tên Kitchen đã tồn tại chưa
            var existingKitchens = await _kitchenRepository.GetAllAsync();

            var duplicatedName = existingKitchens.Any(x =>
                x.KitchenName.Equals(
                    kitchenName,
                    StringComparison.OrdinalIgnoreCase));

            if (duplicatedName)
            {
                throw new BusinessException(
                    "Kitchen name already exists.");
            }

            // Hệ thống chỉ nên có một bếp tổng
            if (request.IsCentralKitchen)
            {
                var centralKitchenExists = existingKitchens.Any(
                    x => x.IsCentralKitchen);

                if (centralKitchenExists)
                {
                    throw new BusinessException(
                        "The system already has a central kitchen.");
                }
            }

            var kitchen = _mapper.Map<Kitchen>(request);
            kitchen.KitchenName = kitchenName;
            kitchen.Address = request.Address?.Trim();
            kitchen.IsActive = true;

            await _kitchenRepository.AddAsync(kitchen);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<KitchenResponse>(kitchen);
        }

        /// <summary>
        /// Cập nhật thông tin Kitchen (tên, địa chỉ).
        /// Chỉ SystemAdmin được cập nhật (kiểm tra ở Controller).
        /// </summary>
        public async Task<KitchenResponse> UpdateAsync(
            int kitchenId,
            UpdateKitchenRequest request)
        {
            var kitchen = await _kitchenRepository
                .GetByIdAsync(kitchenId);

            if (kitchen == null)
            {
                throw new NotFoundException(
                    "Kitchen does not exist.");
            }

            if (!kitchen.IsActive)
            {
                throw new BusinessException(
                    "Inactive kitchen cannot be updated.");
            }

            var newName = request.KitchenName.Trim();

            // Không cho đổi sang tên đang được Kitchen khác sử dụng
            var kitchens = await _kitchenRepository.GetAllAsync();

            var duplicatedName = kitchens.Any(x =>
                x.KitchenId != kitchenId &&
                x.KitchenName.Equals(
                    newName,
                    StringComparison.OrdinalIgnoreCase));

            if (duplicatedName)
            {
                throw new BusinessException(
                    "Kitchen name already exists.");
            }

            _mapper.Map(request, kitchen);
            kitchen.KitchenName = newName;
            kitchen.Address = request.Address?.Trim();

            _kitchenRepository.Update(kitchen);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<KitchenResponse>(kitchen);
        }

        /// <summary>
        /// Khóa Kitchen (xóa mềm).
        /// Không được khóa Bếp tổng vì quy trình điều chuyển phụ thuộc vào đó.
        /// </summary>
        public async Task DeactivateAsync(int kitchenId)
        {
            var kitchen = await _kitchenRepository
                .GetByIdAsync(kitchenId);

            if (kitchen == null)
            {
                throw new NotFoundException(
                    "Kitchen does not exist.");
            }

            if (!kitchen.IsActive)
            {
                return;
            }

            // Không cho vô hiệu hóa bếp tổng
            if (kitchen.IsCentralKitchen)
            {
                throw new BusinessException(
                    "Central kitchen cannot be deactivated.");
            }

            _kitchenRepository.SoftDelete(kitchen);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
