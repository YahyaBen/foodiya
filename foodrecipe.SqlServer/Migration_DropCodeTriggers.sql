-- ═══════════════════════════════════════════════════════════════════════════
-- Migration: Drop all TR_*_GenerateCode triggers
-- ═══════════════════════════════════════════════════════════════════════════
-- WHY:  Code values are now generated in the C# backend (EntityCodeGenerator)
--       BEFORE the INSERT. The AFTER INSERT triggers are redundant and cause
--       EF Core "OUTPUT clause" errors (SQL Server error 334).
--
-- SAFE: Every factory / service already sets Code pre-insert. The triggers
--       were only a fallback safety net.
--
-- ROLLBACK: Re-run Migration_Uuid_Code_AllTables.sql (trigger section).
-- ═══════════════════════════════════════════════════════════════════════════

IF OBJECT_ID(N'[dbo].[TR_AppUser_GenerateCode]', N'TR') IS NOT NULL
    DROP TRIGGER [dbo].[TR_AppUser_GenerateCode];
GO

IF OBJECT_ID(N'[dbo].[TR_AppUserRefreshToken_GenerateCode]', N'TR') IS NOT NULL
    DROP TRIGGER [dbo].[TR_AppUserRefreshToken_GenerateCode];
GO

IF OBJECT_ID(N'[dbo].[TR_AppUserExternalLogin_GenerateCode]', N'TR') IS NOT NULL
    DROP TRIGGER [dbo].[TR_AppUserExternalLogin_GenerateCode];
GO

IF OBJECT_ID(N'[dbo].[TR_ChefProfile_GenerateCode]', N'TR') IS NOT NULL
    DROP TRIGGER [dbo].[TR_ChefProfile_GenerateCode];
GO

IF OBJECT_ID(N'[dbo].[TR_DailyRecipeStats_GenerateCode]', N'TR') IS NOT NULL
    DROP TRIGGER [dbo].[TR_DailyRecipeStats_GenerateCode];
GO

IF OBJECT_ID(N'[dbo].[TR_Ingredient_GenerateCode]', N'TR') IS NOT NULL
    DROP TRIGGER [dbo].[TR_Ingredient_GenerateCode];
GO

IF OBJECT_ID(N'[dbo].[TR_IngredientImage_GenerateCode]', N'TR') IS NOT NULL
    DROP TRIGGER [dbo].[TR_IngredientImage_GenerateCode];
GO

IF OBJECT_ID(N'[dbo].[TR_MoroccanCity_GenerateCode]', N'TR') IS NOT NULL
    DROP TRIGGER [dbo].[TR_MoroccanCity_GenerateCode];
GO

IF OBJECT_ID(N'[dbo].[TR_Recipe_GenerateCode]', N'TR') IS NOT NULL
    DROP TRIGGER [dbo].[TR_Recipe_GenerateCode];
GO

IF OBJECT_ID(N'[dbo].[TR_RecipeImage_GenerateCode]', N'TR') IS NOT NULL
    DROP TRIGGER [dbo].[TR_RecipeImage_GenerateCode];
GO

IF OBJECT_ID(N'[dbo].[TR_RecipeIngredient_GenerateCode]', N'TR') IS NOT NULL
    DROP TRIGGER [dbo].[TR_RecipeIngredient_GenerateCode];
GO

IF OBJECT_ID(N'[dbo].[TR_RecipeShare_GenerateCode]', N'TR') IS NOT NULL
    DROP TRIGGER [dbo].[TR_RecipeShare_GenerateCode];
GO

IF OBJECT_ID(N'[dbo].[TR_RecipeStep_GenerateCode]', N'TR') IS NOT NULL
    DROP TRIGGER [dbo].[TR_RecipeStep_GenerateCode];
GO

PRINT '✅ Migration_DropCodeTriggers: All TR_*_GenerateCode triggers dropped.';
GO
