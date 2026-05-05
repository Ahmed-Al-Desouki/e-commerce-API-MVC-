# MVC Changes Explanation

## Overview

This document explains all changes made in the project so they can be discussed clearly in a presentation, viva, or interview.

The main goal of the work was to improve the `MVC layer` so the application behaves more like a real E-commerce website, while still respecting the existing architecture.

The final user flow became:

`Login -> Browse Products -> Product Details -> Add to Cart -> Checkout`

---

## 1. Changed the Default Startup Route

**File:**
- `ECommerce.Web/Program.cs`

**What changed**
- The default route was changed from `Products/Index` to `Auth/Login`.

**Why**
- Because the required application flow starts from the login page.
- This also improves security and user experience by preventing users from entering the main shopping flow before authentication.

**How to explain it**
- I changed the default MVC route so the application starts at the login page instead of the products page.
- This matches the required flow and makes the application feel more production-like.

---

## 2. Added Session-Based Page Protection

**File:**
- `ECommerce.Web/Filters/RequireSessionAttribute.cs`

**What changed**
- A custom attribute called `RequireSessionAttribute` was added.
- It checks:
  - whether the user is logged in
  - whether the user is an admin when the page is admin-only

**Why**
- To avoid repeating login-check logic in every controller action.
- To keep controllers cleaner.
- To support the current MVC architecture, which stores JWT data in session.

**How to explain it**
- Instead of writing login checks manually in every action, I created a reusable custom filter.
- This makes the code cleaner, easier to maintain, and keeps access rules consistent.

---

## 3. Improved Session Stability

**File:**
- `ECommerce.Web/Program.cs`

**What changed**
- Added `DataProtection` key persistence.
- Added a stable session cookie configuration.

**Why**
- To reduce session cookie decryption issues like:
  - `Error unprotecting the session cookie`
- ASP.NET Core uses data protection keys to encrypt cookies and session values.
- If the keys change between restarts, old cookies may become invalid.

**How to explain it**
- I persisted data protection keys locally so session cookies are more stable across application restarts.
- This prevents avoidable cookie-related warnings and improves reliability.

---

## 4. Improved Login and Register Flow

**Files:**
- `ECommerce.Web/Controllers/AuthController.cs`
- `ECommerce.Web/Views/Auth/Login.cshtml`
- `ECommerce.Web/Views/Auth/Register.cshtml`
- `ECommerce.Web/Models/AuthModels.cs`

**What changed**
- Added validation for email, password, and username.
- Added anti-forgery protection.
- Added graceful handling when the API is offline.
- Stored login result in session:
  - `Token`
  - `Username`
  - `IsAdmin`
  - `UserId`

**Why**
- To avoid raw exceptions on login failure.
- To provide a better user experience.
- To prepare the MVC layer to call secure API endpoints with the JWT token.

**How to explain it**
- The MVC app acts as a client of the API, so after login it stores the JWT token and user info in session.
- This allows the app to send authenticated requests later for cart, orders, and admin actions.

---

## 5. Upgraded the Shared Layout

**Files:**
- `ECommerce.Web/Views/Shared/_Layout.cshtml`
- `ECommerce.Web/wwwroot/css/site.css`

**What changed**
- Redesigned the global layout with Bootstrap 5.
- Added:
  - modern navbar
  - footer
  - alert messages
  - responsive spacing
  - cleaner visual hierarchy

**Why**
- To create a consistent design across all pages.
- To make the application look closer to a production E-commerce frontend.

**How to explain it**
- I centralized the UI improvements in the shared layout so every page inherits the same visual style and navigation structure.

---

## 6. Improved Products Page UI

**File:**
- `ECommerce.Web/Views/Products/Index.cshtml`

**What changed**
- Products are displayed in Bootstrap cards.
- Each card now includes:
  - product image
  - product name
  - stock
  - price
  - details button
  - add-to-cart button
  - edit/delete buttons for admin

**Why**
- Card layout is ideal for E-commerce product presentation.
- It improves readability and user interaction.

**How to explain it**
- I changed the products page from a basic list into a card-based catalog to make browsing easier and more visually appealing.

---

## 7. Added Product Details Page

**Files:**
- `ECommerce.Web/Controllers/ProductsController.cs`
- `ECommerce.Web/Views/Products/Details.cshtml`

**What changed**
- Added a `Details(int id)` action in the MVC controller.
- Added a product details page that shows:
  - image
  - name
  - price
  - stock availability
  - add-to-cart action

**Why**
- To create a natural browsing experience.
- In E-commerce systems, users usually inspect product details before adding to cart.

**How to explain it**
- I added a dedicated details page to improve UX and make the shopping flow more realistic.

---

## 8. Added Product Image Support

**Files:**
- `ECommerce.Web/Services/ProductImageStore.cs`
- `ECommerce.Web/Models/ProductModels.cs`
- `ECommerce.Web/Views/Products/Create.cshtml`
- `ECommerce.Web/Views/Products/Edit.cshtml`
- `ECommerce.Web/Views/Products/Index.cshtml`
- `ECommerce.Web/Views/Products/Details.cshtml`

**What changed**
- Admin can now:
  - upload a product image
  - or provide an image URL
- Images are displayed in:
  - products list
  - product details page

**How it works**
- Uploaded files are stored in:
  - `wwwroot/uploads/products`
- The mapping between `ProductId` and image path is stored in a JSON file in:
  - `App_Data/product-images.json`

**Why**
- The backend product contract initially did not include image persistence.
- The task required image support without breaking architecture or accessing the database directly from MVC.

**How to explain it**
- Since the API did not yet persist product images, I implemented image support inside the MVC layer in a safe and independent way.
- This allowed the frontend to support images without violating clean architecture boundaries.

---

## 9. Added Admin Product Creation Page

**Files:**
- `ECommerce.Web/Views/Products/Create.cshtml`
- `ECommerce.Web/Controllers/ProductsController.cs`

**What changed**
- Added an admin-only create page.
- Added form fields for:
  - name
  - price
  - stock
  - image upload
  - image URL

**Why**
- Product management is a core admin feature in any E-commerce system.
- The UI needed to support it directly from the MVC frontend.

**How to explain it**
- I created an admin form to add products through the API while also supporting optional frontend image handling.

---

## 10. Added Admin Product Editing

**Files:**
- `ECommerce.Web/Views/Products/Edit.cshtml`
- `ECommerce.Web/Controllers/ProductsController.cs`
- `ECommerce.Web/Models/ProductModels.cs`

**What changed**
- Added an admin-only edit page.
- Admin can now update:
  - name
  - price
  - stock
  - image

**Why**
- Full product management should support update operations, not only create and delete.

**How to explain it**
- I added a dedicated edit workflow so admins can manage existing products properly from the MVC layer.

---

## 11. Added Product Update Endpoint in the API

**Files:**
- `ECommerce.API/Controllers/ProductsController.cs`
- `ECommerce.Application/DTOs/Product/UpdateProductDto.cs`
- `ECommerce.Application/Interfaces/Services/IProductService.cs`
- `ECommerce.Application/Services/ProductService.cs`

**What changed**
- Added `PUT /api/products/{id}`
- Added `UpdateProductDto`
- Added `UpdateAsync` in the product service

**Why**
- The MVC layer is only a client.
- If the admin edits a product in MVC, the API must expose a real update endpoint so the change is persisted in the system.

**How to explain it**
- Even though the main focus was MVC, product editing required a minimal API enhancement because the frontend cannot persist product updates by itself.

---

## 12. Kept Business Logic in the Service Layer

**File:**
- `ECommerce.Application/Services/ProductService.cs`

**What changed**
- The update logic was added to the service layer, not inside the controller.

**Why**
- To preserve clean architecture.
- Controllers should only handle requests and responses.
- Business rules belong in the service/application layer.

**How to explain it**
- I kept the update logic in the application service so the controller remains thin and the architecture stays clean.

---

## 13. Improved Cart and Order Pages

**Files:**
- `ECommerce.Web/Controllers/CartController.cs`
- `ECommerce.Web/Controllers/OrderController.cs`
- `ECommerce.Web/Views/Cart/Index.cshtml`
- `ECommerce.Web/Views/Order/Index.cshtml`
- `ECommerce.Web/Models/CartModels.cs`

**What changed**
- Protected cart and orders behind login.
- Improved empty states.
- Added cleaner buttons, tables, and messages.
- Added anti-forgery on actions like checkout and clear cart.

**Why**
- To make the overall user journey consistent from login through checkout.

**How to explain it**
- I improved the surrounding shopping flow so it matches the upgraded products and authentication experience.

---

## 14. Added Validation Using ViewModels

**Files:**
- `ECommerce.Web/Models/AuthModels.cs`
- `ECommerce.Web/Models/ProductModels.cs`
- `ECommerce.Web/Models/CartModels.cs`

**What changed**
- Added data annotations like:
  - `Required`
  - `Range`
  - `StringLength`
  - `EmailAddress`
  - `Url`

**Why**
- To validate form input before sending data to the API.
- To provide clear error messages to users.

**How to explain it**
- I used ViewModels with validation attributes so user input is checked at the MVC layer before any API request is sent.

---

## 15. Added Anti-Forgery Protection

**Files:**
- multiple MVC views and controller actions

**What changed**
- Added:
  - `@Html.AntiForgeryToken()`
  - `[ValidateAntiForgeryToken]`

**Why**
- To protect form submissions against CSRF attacks.

**How to explain it**
- Since the MVC app contains forms for login, create, edit, delete, cart, and checkout, anti-forgery protection was added to improve security.

---

## 16. Graceful API Failure Handling

**File:**
- `ECommerce.Web/Controllers/AuthController.cs`

**What changed**
- Added `try/catch` around API calls in login/register.
- If the API is offline, the user sees a friendly message instead of a server crash.

**Why**
- The MVC app depends on the API.
- If the API is unavailable, the frontend should fail gracefully.

**How to explain it**
- I improved resilience by handling connection failures and showing user-friendly feedback instead of throwing an unhandled exception.

---

## 17. Build Verification

**What was verified**
- `ECommerce.Web` build passed
- `ECommerce.API` build passed

**Note**
- There is an existing warning in:
  - `ECommerce.Infrastructure/Repositories/ProductRepository.cs`
- The method `DeleteAsync` is marked `async` without `await`.

**How to explain it**
- It is a code quality warning, not a functional error.
- The application still builds and works correctly.

---

## Short Summary for Discussion

If someone asks for a quick summary, you can say:

1. I improved the MVC frontend to start from login and follow a full shopping flow.
2. I added session-based protection for user and admin pages.
3. I redesigned the UI using Bootstrap 5 with better layout and responsiveness.
4. I added product details, empty states, and better validation.
5. I added product image support in the MVC layer.
6. I added admin product management: create, edit, and delete.
7. I added a minimal update endpoint in the API so admin editing works correctly.
8. I improved security with anti-forgery and better error handling.

---

## Common Questions and Answers

### Why did you store JWT in Session?
- Because the MVC application acts as a client for the API.
- After login, the token is needed for later authenticated requests such as cart, orders, and admin operations.

### Why did you create a custom session filter?
- To avoid repeating login checks in every action.
- To keep the code cleaner and more maintainable.

### Why did you use ViewModels?
- To separate UI input handling from API and domain models.
- To apply validation rules in the MVC layer.

### Why did you add a PUT endpoint in the API if the task focused on MVC?
- Because MVC alone cannot persist product edits.
- A minimal API update endpoint was necessary for real edit functionality.

### Why were product images handled in MVC instead of the database?
- Because the backend contract initially did not support storing image fields.
- The chosen solution added image support without violating architecture boundaries.

### Why did you use TempData?
- To show one-time success or error messages after redirects.

### Why did you add anti-forgery tokens?
- To protect form requests from CSRF attacks.

---

## Final Talking Point

The most important thing to say is:

The architecture was respected.  
The MVC layer remained a client to the API.  
The UI and UX were significantly improved.  
And only the minimum backend changes required for proper product editing were added.
