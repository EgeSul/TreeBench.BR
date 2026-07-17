--USE master;
--GO

--IF EXISTS (SELECT * FROM sys.databases WHERE name = 'TreeBenchDB')
--BEGIN
--    ALTER DATABASE TreeBenchDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
--    DROP DATABASE TreeBenchDB;
--END
--GO

--CREATE DATABASE TreeBenchDB;
--GO

--USE TreeBenchDB;
--GO

---- 2. Test verilerini tutacağımız tablo
--CREATE TABLE TestNumbers (
--    ID INT IDENTITY(1,1) PRIMARY KEY,
--    Value INT NOT NULL CONSTRAINT UC_TestNumbers_Value UNIQUE, 
--    DataType VARCHAR(20) NOT NULL -- 'Random' OR 'Sorted' 
--);
--GO

--SET NOCOUNT ON; 

--PRINT '[>] 100.000 Rastgele veri üretimi başlatıldı... Biraz sürebilir.';

--DECLARE @Counter INT = 1;
--DECLARE @RandomValue INT;

--WHILE @Counter <= 100000
--BEGİN
--    SET @RandomValue = FLOOR(RAND() * 1000000) + 1;

--    IF NOT EXISTS (SELECT 1 FROM TestNumbers WHERE Value = @RandomValue)
--    BEGIN
--        INSERT INTO TestNumbers (Value, DataType) 
--        VALUES (@RandomValue, 'Random');
        
--        SET @Counter = @Counter + 1;
--    END
--END

--PRINT '[+] 100.000 Rastgele veri başarıyla yüklendi!';
--GO

--SELECT DataType, COUNT(*) AS ToplamEleman 
--FROM TestNumbers 
--GROUP BY DataType;
--GO

--select * from TestNumbers;