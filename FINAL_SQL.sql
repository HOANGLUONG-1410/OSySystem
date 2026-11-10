BEGIN TRANSACTION;

BEGIN TRY

    -------------------------------------------------------------------
    -- 0. LẤY ACCOUNT ADMIN ĐÃ ĐƯỢC TẠO BỞI DatabaseSeeder
    -------------------------------------------------------------------

    DECLARE @AccountId INT;

    SELECT TOP 1
        @AccountId = AccountId
    FROM SystemAccounts
    WHERE Username = 'admin'
      AND IsActive = 1;

    IF @AccountId IS NULL
    BEGIN
        THROW 50001,
        'Không tìm thấy tài khoản admin. Hãy chạy DatabaseSeeder trước.',
        1;
    END;


    -------------------------------------------------------------------
    -- 1. KITCHENS
    -------------------------------------------------------------------

    DECLARE @Kitchen1Id INT;
    DECLARE @Kitchen2Id INT;

    INSERT INTO Kitchens
    (
        KitchenName,
        Address,
        IsCentralKitchen,
        IsActive
    )
    VALUES
    (
        N'Bếp Tổng OSỹ - Hà Nội',
        N'123 Nguyễn Trãi, Thanh Xuân, Hà Nội',
        1,
        1
    );

    SET @Kitchen1Id = SCOPE_IDENTITY();


    INSERT INTO Kitchens
    (
        KitchenName,
        Address,
        IsCentralKitchen,
        IsActive
    )
    VALUES
    (
        N'Chi nhánh Bún Bò Huế OSỹ - Cầu Giấy',
        N'45 Trần Thái Tông, Cầu Giấy, Hà Nội',
        0,
        1
    );

    SET @Kitchen2Id = SCOPE_IDENTITY();


    -------------------------------------------------------------------
    -- 2. INGREDIENTS
    -------------------------------------------------------------------

    DECLARE @IngThitBoId INT;
    DECLARE @IngBunId INT;
    DECLARE @IngChaId INT;


    INSERT INTO Ingredients
    (
        IngredientName,
        Unit,
        AverageCost,
        IsActive
    )
    VALUES
    (
        N'Thịt bò nạm',
        N'kg',
        180000.00,
        1
    );

    SET @IngThitBoId = SCOPE_IDENTITY();


    INSERT INTO Ingredients
    (
        IngredientName,
        Unit,
        AverageCost,
        IsActive
    )
    VALUES
    (
        N'Bún tươi',
        N'kg',
        12000.00,
        1
    );

    SET @IngBunId = SCOPE_IDENTITY();


    INSERT INTO Ingredients
    (
        IngredientName,
        Unit,
        AverageCost,
        IsActive
    )
    VALUES
    (
        N'Chả cây Huế',
        N'Cái',
        5000.00,
        1
    );

    SET @IngChaId = SCOPE_IDENTITY();


    -------------------------------------------------------------------
    -- 3. MENU ITEMS
    -------------------------------------------------------------------

    DECLARE @MenuBunDacBietId INT;
    DECLARE @MenuBunThuongId INT;


    INSERT INTO MenuItems
    (
        MenuItemName,
        SellingPrice,
        IsActive
    )
    VALUES
    (
        N'Bún bò Huế đặc biệt',
        65000.00,
        1
    );

    SET @MenuBunDacBietId = SCOPE_IDENTITY();


    INSERT INTO MenuItems
    (
        MenuItemName,
        SellingPrice,
        IsActive
    )
    VALUES
    (
        N'Bún bò Huế thường',
        50000.00,
        1
    );

    SET @MenuBunThuongId = SCOPE_IDENTITY();


    -------------------------------------------------------------------
    -- 4. RECIPES
    -------------------------------------------------------------------

    -- Bún bò Huế đặc biệt:
    -- 0.150 kg thịt bò
    -- 0.200 kg bún
    -- 2 cái chả

    INSERT INTO Recipes
    (
        MenuItemId,
        IngredientId,
        QuantityRequired
    )
    VALUES
    (
        @MenuBunDacBietId,
        @IngThitBoId,
        0.150
    );


    INSERT INTO Recipes
    (
        MenuItemId,
        IngredientId,
        QuantityRequired
    )
    VALUES
    (
        @MenuBunDacBietId,
        @IngBunId,
        0.200
    );


    INSERT INTO Recipes
    (
        MenuItemId,
        IngredientId,
        QuantityRequired
    )
    VALUES
    (
        @MenuBunDacBietId,
        @IngChaId,
        2.000
    );


    -- Bún bò Huế thường:
    -- 0.100 kg thịt bò
    -- 0.200 kg bún

    INSERT INTO Recipes
    (
        MenuItemId,
        IngredientId,
        QuantityRequired
    )
    VALUES
    (
        @MenuBunThuongId,
        @IngThitBoId,
        0.100
    );


    INSERT INTO Recipes
    (
        MenuItemId,
        IngredientId,
        QuantityRequired
    )
    VALUES
    (
        @MenuBunThuongId,
        @IngBunId,
        0.200
    );


    -------------------------------------------------------------------
    -- 5. INVENTORIES
    --
    -- LastUpdated là NOT NULL nên phải truyền giá trị.
    -------------------------------------------------------------------

    -- Bếp Tổng

    INSERT INTO Inventories
    (
        KitchenId,
        IngredientId,
        Quantity,
        LastUpdated
    )
    VALUES
    (
        @Kitchen1Id,
        @IngThitBoId,
        100.000,
        GETDATE()
    );


    INSERT INTO Inventories
    (
        KitchenId,
        IngredientId,
        Quantity,
        LastUpdated
    )
    VALUES
    (
        @Kitchen1Id,
        @IngBunId,
        200.000,
        GETDATE()
    );


    INSERT INTO Inventories
    (
        KitchenId,
        IngredientId,
        Quantity,
        LastUpdated
    )
    VALUES
    (
        @Kitchen1Id,
        @IngChaId,
        500.000,
        GETDATE()
    );


    -- Chi nhánh

    INSERT INTO Inventories
    (
        KitchenId,
        IngredientId,
        Quantity,
        LastUpdated
    )
    VALUES
    (
        @Kitchen2Id,
        @IngThitBoId,
        15.000,
        GETDATE()
    );


    INSERT INTO Inventories
    (
        KitchenId,
        IngredientId,
        Quantity,
        LastUpdated
    )
    VALUES
    (
        @Kitchen2Id,
        @IngBunId,
        30.000,
        GETDATE()
    );


    INSERT INTO Inventories
    (
        KitchenId,
        IngredientId,
        Quantity,
        LastUpdated
    )
    VALUES
    (
        @Kitchen2Id,
        @IngChaId,
        50.000,
        GETDATE()
    );


    -------------------------------------------------------------------
    -- 6. INVENTORY LOGS
    --
    -- Log nhập kho ban đầu
    -------------------------------------------------------------------

    INSERT INTO InventoryLogs
    (
        KitchenId,
        IngredientId,
        LogType,
        QuantityChange,
        QuantityBefore,
        QuantityAfter,
        Note,
        PerformedByAccountId,
        CreatedAt
    )
    VALUES
    (
        @Kitchen1Id,
        @IngThitBoId,
        0,
        100.000,
        0.000,
        100.000,
        N'Nhập tồn kho ban đầu - Bếp Tổng',
        @AccountId,
        GETDATE()
    );


    INSERT INTO InventoryLogs
    (
        KitchenId,
        IngredientId,
        LogType,
        QuantityChange,
        QuantityBefore,
        QuantityAfter,
        Note,
        PerformedByAccountId,
        CreatedAt
    )
    VALUES
    (
        @Kitchen2Id,
        @IngThitBoId,
        0,
        15.000,
        0.000,
        15.000,
        N'Nhập tồn kho ban đầu - Chi nhánh',
        @AccountId,
        GETDATE()
    );


    -------------------------------------------------------------------
    -- 7. ORDER PENDING
    --
    -- Dùng để test:
    --
    -- Pending
    --     ↓
    -- Confirm
    --     ↓
    -- Confirmed
    --     ↓
    -- Trừ Inventory
    --     ↓
    -- Ghi InventoryLog
    -------------------------------------------------------------------

    DECLARE @OrderId1 INT;


    INSERT INTO Orders
    (
        KitchenId,
        OrderDate,
        Status,
        TotalAmount,
        CreatedByAccountId
    )
    VALUES
    (
        @Kitchen2Id,
        GETDATE(),
        0,
        130000.00,
        @AccountId
    );

    SET @OrderId1 = SCOPE_IDENTITY();


    INSERT INTO OrderDetails
    (
        OrderId,
        MenuItemId,
        Quantity,
        UnitPrice,
        SubTotal
    )
    VALUES
    (
        @OrderId1,
        @MenuBunDacBietId,
        2,
        65000.00,
        130000.00
    );


    -------------------------------------------------------------------
    -- 8. ORDER COMPLETED
    --
    -- Dùng để test Dashboard có dữ liệu Completed.
    --
    -- Lưu ý:
    -- Đây là dữ liệu mẫu được tạo trực tiếp bằng SQL.
    -- Không nên dùng record này để kiểm tra logic ConfirmOrder,
    -- vì ConfirmOrderService mới là nơi thực hiện trừ kho.
    -------------------------------------------------------------------

    DECLARE @OrderId2 INT;


    INSERT INTO Orders
    (
        KitchenId,
        OrderDate,
        Status,
        TotalAmount,
        CreatedByAccountId
    )
    VALUES
    (
        @Kitchen2Id,
        GETDATE(),
        2,
        50000.00,
        @AccountId
    );

    SET @OrderId2 = SCOPE_IDENTITY();


    INSERT INTO OrderDetails
    (
        OrderId,
        MenuItemId,
        Quantity,
        UnitPrice,
        SubTotal
    )
    VALUES
    (
        @OrderId2,
        @MenuBunThuongId,
        1,
        50000.00,
        50000.00
    );


    -------------------------------------------------------------------
    -- 9. ADJUSTMENT SAMPLE
    --
    -- Đây là dữ liệu lịch sử để Dashboard / danh sách Adjustment
    -- có dữ liệu hiển thị.
    --
    -- Không dùng record này để test AdjustmentService.
    -------------------------------------------------------------------

    INSERT INTO Adjustments
    (
        KitchenId,
        IngredientId,
        SystemQuantity,
        ActualQuantity,
        DifferenceQuantity,
        Reason,
        CreatedByAccountId,
        CreatedAt
    )
    VALUES
    (
        @Kitchen2Id,
        @IngThitBoId,
        15.000,
        13.500,
        -1.500,
        N'Hao hụt tự nhiên khi sơ chế',
        @AccountId,
        GETDATE()
    );


    -------------------------------------------------------------------
    -- 10. TRANSFER REQUEST - DRAFT
    --
    -- Dùng để test:
    --
    -- Draft
    --   ↓
    -- Send
    --   ↓
    -- InTransit
    --   ↓
    -- Trừ kho Bếp Tổng
    --   ↓
    -- Receive
    --   ↓
    -- Cộng kho Chi Nhánh
    -------------------------------------------------------------------

    DECLARE @TransferId INT;


    INSERT INTO TransferRequests
    (
        FromKitchenId,
        ToKitchenId,
        Status,
        Note,
        CreatedByAccountId,
        CreatedAt
    )
    VALUES
    (
        @Kitchen1Id,
        @Kitchen2Id,
        0,
        N'Cấp ứng nguyên liệu thịt bò cho chi nhánh',
        @AccountId,
        GETDATE()
    );

    SET @TransferId = SCOPE_IDENTITY();


    INSERT INTO TransferRequestDetails
    (
        TransferRequestId,
        IngredientId,
        Quantity
    )
    VALUES
    (
        @TransferId,
        @IngThitBoId,
        10.000
    );


    -------------------------------------------------------------------
    -- COMMIT
    -------------------------------------------------------------------

    COMMIT TRANSACTION;

    PRINT N'================================================';
    PRINT N'TẠO DATA MẪU THÀNH CÔNG!';
    PRINT N'================================================';

END TRY

BEGIN CATCH

    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END;

    PRINT N'================================================';
    PRINT N'TẠO DATA MẪU THẤT BẠI!';
    PRINT N'================================================';

    THROW;

END CATCH;

SELECT * FROM Kitchens;
SELECT * FROM Ingredients;
SELECT * FROM MenuItems;
SELECT * FROM Recipes;
SELECT * FROM Inventories;
SELECT * FROM InventoryLogs;
SELECT * FROM Orders;
SELECT * FROM OrderDetails;
SELECT * FROM Adjustments;
SELECT * FROM TransferRequests;
SELECT * FROM TransferRequestDetails;