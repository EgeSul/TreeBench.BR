-- 1. Veri tabanını sıfırdan oluştur
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
--    Value INT NOT NULL CONSTRAINT UC_TestNumbers_Value UNIQUE, -- Ağaçların kafası karışmasın diye benzersiz (Unique)
--    DataType VARCHAR(20) NOT NULL -- 'Random' veya 'Sorted' ayrımı yapmak için
--);
--GO

---- 3. PERFORMANS İÇİN LOOP MOTORU (100.000 Rastgele Veri)
--SET NOCOUNT ON; -- Konsolu gereksiz loglarla yorup işlemi yavaşlatmasın

--PRINT '[>] 100.000 Rastgele veri üretimi başlatıldı... Biraz sürebilir.';

--DECLARE @Counter INT = 1;
--DECLARE @RandomValue INT;

--WHILE @Counter <= 100000
--BEGIN
--    -- 1 ile 1.000.000 arasında rastgele bir tam sayı üret
--    SET @RandomValue = FLOOR(RAND() * 1000000) + 1;

--    -- Eğer bu sayı daha önce eklenmediyse tabloya ekle
--    IF NOT EXISTS (SELECT 1 FROM TestNumbers WHERE Value = @RandomValue)
--    BEGIN
--        INSERT INTO TestNumbers (Value, DataType) 
--        VALUES (@RandomValue, 'Random');
        
--        SET @Counter = @Counter + 1;
--    END
--END

--PRINT '[+] 100.000 Rastgele veri başarıyla yüklendi!';
--GO

---- 4. Kontrol Sorgusu (Veriler geldi mi diye bakmak için)
--SELECT DataType, COUNT(*) AS ToplamEleman 
--FROM TestNumbers 
--GROUP BY DataType;
--GO

--select * from TestNumbers;