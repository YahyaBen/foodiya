-- ============================================================
-- Migration: Set placeholder image URL on all referential tables
-- Date: 2026-04-10
-- ============================================================

DECLARE @Url NVARCHAR(500) = N'https://ressourcesone9664.blob.core.windows.net/foodiya-images/cuisines/af9a5f03-51cf-48fe-b8e9-4362439db930.png';

-- Tables that already have IconUrl
UPDATE Cuisine         SET IconUrl = @Url WHERE IconUrl IS NULL;
UPDATE Difficulty      SET IconUrl = @Url WHERE IconUrl IS NULL;
UPDATE FoodCategory    SET IconUrl = @Url WHERE IconUrl IS NULL;
UPDATE IngredientType  SET IconUrl = @Url WHERE IconUrl IS NULL;

-- Tables that have ImageUrl / CoverImageUrl / ProfileImageUrl
UPDATE Recipe          SET CoverImageUrl   = @Url WHERE CoverImageUrl   IS NULL;
UPDATE RecipeImage     SET ImageUrl         = @Url WHERE ImageUrl        IS NULL;
UPDATE PresetAvatarImage SET ImageUrl       = @Url WHERE ImageUrl        IS NULL;
UPDATE AppUser         SET ProfileImageUrl  = @Url WHERE ProfileImageUrl IS NULL;

-- IngredientImage child table
UPDATE IngredientImage SET ImageUrl = @Url WHERE ImageUrl IS NULL;

PRINT 'Done — placeholder image set on all NULL image fields.';
