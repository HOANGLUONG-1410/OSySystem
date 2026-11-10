using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enums
{
    public enum OrderStatus
    {
        // Nhân viên vừa tạo đơn, chưa tác động kho
        Pending = 0,

        // Bếp đã xác nhận, hệ thống đã trừ kho
        Confirmed = 1,

        // Đơn đã hoàn thành và giao cho khách
        Completed = 2,

        // Đơn bị hủy khi chưa xác nhận
        Cancelled = 3
    }
}
