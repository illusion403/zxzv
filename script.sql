-- ============================================================
-- FULL COLUMN INVENTORY (MySQL)
-- ============================================================

-- =========================
-- 0. HEADER
-- =========================
SELECT '===== START COLUMN ANALYSIS =====' AS section, NOW() AS run_time;

-- =========================
-- 1. ALL COLUMNS (FULL DETAIL)
-- =========================
SELECT '===== ALL COLUMNS =====' AS section;

SELECT 
    c.TABLE_SCHEMA,
    c.TABLE_NAME,
    c.COLUMN_NAME,
    c.ORDINAL_POSITION,
    c.COLUMN_TYPE,
    c.DATA_TYPE,
    c.CHARACTER_MAXIMUM_LENGTH,
    c.NUMERIC_PRECISION,
    c.NUMERIC_SCALE,
    c.IS_NULLABLE,
    c.COLUMN_DEFAULT,
    c.EXTRA,
    c.COLUMN_KEY,
    c.COLUMN_COMMENT
FROM information_schema.COLUMNS c
ORDER BY c.TABLE_SCHEMA, c.TABLE_NAME, c.ORDINAL_POSITION;

-- =========================
-- 2. PRIMARY KEYS
-- =========================
SELECT '===== PRIMARY KEYS =====' AS section;

SELECT 
    TABLE_SCHEMA,
    TABLE_NAME,
    COLUMN_NAME
FROM information_schema.KEY_COLUMN_USAGE
WHERE CONSTRAINT_NAME = 'PRIMARY'
ORDER BY TABLE_SCHEMA, TABLE_NAME;

-- =========================
-- 3. FOREIGN KEYS
-- =========================
SELECT '===== FOREIGN KEYS =====' AS section;

SELECT 
    kcu.TABLE_SCHEMA,
    kcu.TABLE_NAME,
    kcu.COLUMN_NAME,
    kcu.REFERENCED_TABLE_NAME,
    kcu.REFERENCED_COLUMN_NAME
FROM information_schema.KEY_COLUMN_USAGE kcu
WHERE kcu.REFERENCED_TABLE_NAME IS NOT NULL
ORDER BY kcu.TABLE_SCHEMA, kcu.TABLE_NAME;

-- =========================
-- 4. UNIQUE KEYS
-- =========================
SELECT '===== UNIQUE KEYS =====' AS section;

SELECT 
    TABLE_SCHEMA,
    TABLE_NAME,
    COLUMN_NAME,
    CONSTRAINT_NAME
FROM information_schema.KEY_COLUMN_USAGE
WHERE CONSTRAINT_NAME != 'PRIMARY'
AND CONSTRAINT_NAME IN (
    SELECT CONSTRAINT_NAME
    FROM information_schema.TABLE_CONSTRAINTS
    WHERE CONSTRAINT_TYPE = 'UNIQUE'
);

-- =========================
-- 5. AUTO INCREMENT COLUMNS
-- =========================
SELECT '===== AUTO INCREMENT =====' AS section;

SELECT 
    TABLE_SCHEMA,
    TABLE_NAME,
    COLUMN_NAME
FROM information_schema.COLUMNS
WHERE EXTRA LIKE '%auto_increment%';

-- =========================
-- 6. NULLABLE CHECK
-- =========================
SELECT '===== NULLABLE COLUMNS =====' AS section;

SELECT 
    TABLE_SCHEMA,
    TABLE_NAME,
    COLUMN_NAME
FROM information_schema.COLUMNS
WHERE IS_NULLABLE = 'YES';

-- =========================
-- 7. COLUMN COUNT PER TABLE
-- =========================
SELECT '===== COLUMN COUNT =====' AS section;

SELECT 
    TABLE_SCHEMA,
    TABLE_NAME,
    COUNT(*) AS total_columns
FROM information_schema.COLUMNS
GROUP BY TABLE_SCHEMA, TABLE_NAME
ORDER BY total_columns DESC;

-- =========================
-- 8. WIDE TABLES (Danger Zone)
-- =========================
SELECT '===== WIDE TABLES (>50 columns) =====' AS section;

SELECT 
    TABLE_SCHEMA,
    TABLE_NAME,
    COUNT(*) AS column_count
FROM information_schema.COLUMNS
GROUP BY TABLE_SCHEMA, TABLE_NAME
HAVING COUNT(*) > 50
ORDER BY column_count DESC;

-- =========================
-- 9. GENERATE SELECT TEMPLATE
-- =========================
SELECT '===== GENERATE SELECT =====' AS section;

SELECT 
    TABLE_SCHEMA,
    TABLE_NAME,
    CONCAT(
        'SELECT ',
        GROUP_CONCAT(COLUMN_NAME ORDER BY ORDINAL_POSITION SEPARATOR ', '),
        ' FROM `', TABLE_SCHEMA, '`.`', TABLE_NAME, '`;'
    ) AS select_sql
FROM information_schema.COLUMNS
GROUP BY TABLE_SCHEMA, TABLE_NAME;

-- =========================
-- 10. GENERATE INSERT TEMPLATE
-- =========================
SELECT '===== GENERATE INSERT TEMPLATE =====' AS section;

SELECT 
    TABLE_SCHEMA,
    TABLE_NAME,
    CONCAT(
        'INSERT INTO `', TABLE_SCHEMA, '`.`', TABLE_NAME, '` (',
        GROUP_CONCAT(COLUMN_NAME ORDER BY ORDINAL_POSITION SEPARATOR ', '),
        ') VALUES (...);'
    ) AS insert_template
FROM information_schema.COLUMNS
GROUP BY TABLE_SCHEMA, TABLE_NAME;

-- =========================
-- 11. END
-- =========================
SELECT '===== END COLUMN ANALYSIS =====' AS section, NOW() AS end_time;
