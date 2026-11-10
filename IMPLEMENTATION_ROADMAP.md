# PROJECT AUDIT \& IMPLEMENTATION ROADMAP

## ✅ PHASE 1: COMPLETED

* \[x] Created custom exception classes (AuthenticationException, ForbiddenException, BusinessException, NotFoundException)
* \[x] Created Global Exception Middleware with proper HTTP status code mapping
* \[x] Updated CurrentUserService to use AuthenticationException
* \[x] Updated Program.cs to register ExceptionMiddleware

## 🔄 PHASE 2: PENDING - FIX ALL SERVICES (Replace UnauthorizedAccessException with custom exceptions)

### Files to Update:

1. **AuthService.cs** - Replace UnauthorizedAccessException → AuthenticationException
2. **InventoryService.cs** - Replace exceptions and add custom ones
3. **OrderService.cs** - Replace exceptions
4. **TransferService.cs** - Replace exceptions
5. **AdjustmentService.cs** - Replace exceptions
6. **AccountService.cs** - Replace exceptions
7. **KitchenService.cs** - Replace exceptions
8. **IngredientService.cs** - Replace exceptions
9. **MenuItemService.cs** - Replace exceptions
10. **RecipeService.cs** - Replace exceptions

### Replacement Rules:

* `KeyNotFoundException` → `NotFoundException` (404)
* `UnauthorizedAccessException` (permission related) → `ForbiddenException` (403)
* `UnauthorizedAccessException` (auth related) → `AuthenticationException` (401)
* `InvalidOperationException` (business rule) → `BusinessException` (400)
* `ArgumentException` → Keep but middleware will map to 400

\---

## 🔄 PHASE 3: PENDING - FIX ALL DAOs (Add AsNoTracking() \& Include)

### Files to Update:

1. **InventoryDAO.cs**

   * GetByKitchenAndIngredientAsync: Add AsNoTracking()
   * GetByKitchenIdAsync: Include Kitchen for mapping
2. **OrderDAO.cs**

   * GetByIdAsync: Include CreatedByAccount
   * GetAllAsync: Include CreatedByAccount
   * GetByKitchenIdAsync: Include CreatedByAccount
   * GetByKitchenAndStatusAsync: Include CreatedByAccount
   * All read queries: Add AsNoTracking()
3. **InventoryLogDAO.cs**

   * All methods: Include PerformedByAccount
   * All read queries: Add AsNoTracking()
4. **TransferRequestDAO.cs**

   * GetByIdAsync: Include CreatedByAccount, SentByAccount, ReceivedByAccount
   * GetAllAsync: Include all account references
   * All read queries: Add AsNoTracking()
5. **AdjustmentDAO.cs**

   * All methods: Include CreatedByAccount, Kitchen, Ingredient
   * All read queries: Add AsNoTracking()
6. **AllOther DAOs** - Audit for AsNoTracking() usage

\---

## 🔄 PHASE 4: PENDING - FIX Response DTOs (Add missing fields)

### Files to Add Fields:

1. **InventoryLogResponse.cs**

```csharp
   public int? PerformedByAccountId { get; set; }
   public string? PerformedByName { get; set; }
   ```

2. **OrderResponse.cs**

```csharp
   public int? CreatedByAccountId { get; set; }
   public string? CreatedByName { get; set; }
   ```

3. **TransferRequestResponse.cs**

```csharp
   public int? CreatedByAccountId { get; set; }
   public string? CreatedByName { get; set; }
   
   public int? SentByAccountId { get; set; }
   public string? SentByName { get; set; }
   
   public int? ReceivedByAccountId { get; set; }
   public string? ReceivedByName { get; set; }
   ```

4. **AdjustmentResponse.cs**

```csharp
   public int? CreatedByAccountId { get; set; }
   public string? CreatedByName { get; set; }
   ```

\---

## 🔄 PHASE 5: PENDING - FIX ALL SERVICES (Update mapping methods \& exception handling)

### Mapping Functions to Update:

* MapInventoryLogToResponse: Include PerformedByAccountId, PerformedByName
* MapOrderToResponse: Include CreatedByAccountId, CreatedByName
* All similar mapping functions

### Exception Handling:

* Replace all UnauthorizedAccessException with appropriate custom exceptions
* Replace all KeyNotFoundException with NotFoundException
* Replace all InvalidOperationException with BusinessException (where appropriate)

\---

## 🔄 PHASE 6: PENDING - FIX ALL CONTROLLERS (Simplify error handling)

### Remove manual try-catch blocks and use middleware

### Current Pattern (REMOVE):

```csharp
catch (KeyNotFoundException ex)
{
    return NotFound(new { message = ex.Message });
}
catch (InvalidOperationException ex)
{
    return BadRequest(new { message = ex.Message });
}
```

### New Pattern (KEEP SIMPLE):

```csharp
// No try-catch needed - middleware handles all exceptions
var result = await \_service.DoSomethingAsync();
return Ok(result);
```

\---

## 🔄 PHASE 7: PENDING - ADD PAGINATION \& FILTER DTOs

### New Files:

1. **Services/DTOs/Common/PagedRequest.cs**
2. **Services/DTOs/Common/PagedResponse.cs**

### Update Services:

* Add paged overload methods
* Implement Skip() and Take() in DAOs
* Never paginate in-memory

\---

## 🔄 PHASE 8: PENDING - CREATE DATABASE SEEDER

### Seeds to Add:

1. Kitchen (Central + Branches)
2. SystemAccount (all roles)
3. Ingredient (sample items)
4. MenuItem (sample menu)
5. Recipe (connect MenuItem to Ingredient)
6. Inventory (initial stock)

\---

## 🔄 PHASE 9: PENDING - VERIFY BUILD \& TESTS

* \[ ] Build solution
* \[ ] Fix compilation errors
* \[ ] Run unit tests if available

\---

## CRITICAL FILES SUMMARY

|Priority|File|Action|Status|
|-|-|-|-|
|CRITICAL|Services/Exceptions/\*.cs|CREATE|✅ DONE|
|CRITICAL|Web\_API/Middlewares/ExceptionMiddleware.cs|CREATE|✅ DONE|
|CRITICAL|Web\_API/Program.cs|UPDATE|✅ DONE|
|HIGH|All Services (10 files)|REPLACE EXCEPTIONS|⏳ TODO|
|HIGH|All DAOs (12 files)|ADD AsNoTracking + Include|⏳ TODO|
|HIGH|All Response DTOs (5+ files)|ADD MISSING FIELDS|⏳ TODO|
|HIGH|All Controllers (10 files)|REMOVE manual error handling|⏳ TODO|
|MEDIUM|Repository/DAO Methods|ADD missing queries|⏳ TODO|
|MEDIUM|Common DTOs|CREATE Pagination classes|⏳ TODO|
|MEDIUM|DatabaseSeeder|COMPLETE implementation|⏳ TODO|

\---

## NEXT STEPS

Run the build to identify any immediate compilation errors:

```
dotnet build
```

Then proceed with fixing each phase in order.

