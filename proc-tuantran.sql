--=================================================================================================--
-- CAP NHAT DIEM CHO KHACH HANG
GO
CREATE OR ALTER PROCEDURE SP_TANGDIEMKHACHHANG @maKH VARCHAR(10), @diem SMALLINT
AS
BEGIN TRAN
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	DECLARE @temp SMALLINT;
	IF(@diem < 0)
	BEGIN
	PRINT(N'Vui long nhap lai')
		RETURN
	END
		
	IF(NOT EXISTS(SELECT * FROM KHACH_HANG WHERE MAKHACHHANG = @maKH))
	BEGIN
		PRINT(N'Khong co khach hang vua nhap. Vui long nhap lai.')
		RETURN
	END
	--Them diem hoat dong
	SET @temp=(SELECT DIEM_HOAT_DONG FROM KHACH_HANG WHERE MAKHACHHANG = @maKH)
	WAITFOR DELAY '00:00:05'
	SET @temp=@temp+@diem
	UPDATE KHACH_HANG
	SET DIEM_HOAT_DONG=@temp
	WHERE MAKHACHHANG = @maKH
COMMIT TRAN
GO

--=================================================================================================--
-- TANG DIEM KHI KHACH HANG MUA 1 NHA
CREATE OR ALTER PROCEDURE SP_TANGDIEMKHMUANHA @sdt VARCHAR(10)
AS
BEGIN
DECLARE @temp SMALLINT;		
	IF(NOT EXISTS(SELECT * FROM KHACH_HANG WHERE DIENTHOAIKHACHHANG = @sdt))
	BEGIN
		PRINT(N'Khong co khach hang vua nhap. Vui long nhap lai.')
		RETURN
	END
	--Them diem hoat dong
	SET @temp=(SELECT DIEM_HOAT_DONG FROM KHACH_HANG WHERE DIENTHOAIKHACHHANG = @sdt)
	SET @temp=@temp+100
	UPDATE KHACH_HANG
	SET DIEM_HOAT_DONG=@temp
	WHERE MAKHACHHANG = @sdt
END;
GO	

--=================================================================================================--
--THONG KE KHACH HANG TIEM NANG
CREATE OR ALTER PROC SP_THONGKEKHTIEMNANG
AS
BEGIN TRAN
	 SET TRANSACTION ISOLATION LEVEL REPEATABLE READ
	SELECT MAKHACHHANG,DIEM_HOAT_DONG FROM KHACH_HANG WHERE DIEM_HOAT_DONG >=700
	SELECT COUNT(*)AS SOLUONGKHTIEMNANG FROM KHACH_HANG WHERE DIEM_HOAT_DONG >=700
	 WAITFOR DELAY'00:00:05'
	 SELECT COUNT(*)AS SOLUONGKHTIEMNANG FROM KHACH_HANG WHERE DIEM_HOAT_DONG >=700
COMMIT TRAN
go

--=================================================================================================--
--CAP NHAT SUA TEN KHACH HANG
CREATE OR ALTER PROC SP_UPDATENAMEKH @maKH VARCHAR(10), @name VARCHAR(50)
as
BEGIN TRAN
DECLARE @temp VARCHAR(50)
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ
IF(NOT EXISTS(SELECT * FROM KHACH_HANG WHERE MAKHACHHANG = @maKH))
	BEGIN
		PRINT(N'Khong co khach hang vua nhap. Vui long nhap lai.')
		RETURN
	END
	SET @temp=(SELECT TENKHACHHANG FROM KHACH_HANG WHERE MAKHACHHANG=@maKH)
	 SET @temp = @name
UPDATE KHACH_HANG
	SET TENKHACHHANG=@temp
	WHERE MAKHACHHANG = @maKH
COMMIT TRAN

--=================FIX=======================================================================

--=================================================================================================--
-- CAP NHAT DIEM CHO KHACH HANG
GO
CREATE OR ALTER PROCEDURE SP_TANGDIEMKHACHHANG_fix @maKH VARCHAR(10), @diem SMALLINT
AS
BEGIN TRAN
SET TRANSACTION ISOLATION LEVEL  REPEATABLE READ
	DECLARE @temp SMALLINT;
	IF(@diem < 0)
	BEGIN
	PRINT(N'Vui long nhap lai')
		RETURN
	END
		
	IF(NOT EXISTS(SELECT * FROM KHACH_HANG  WHERE MAKHACHHANG = @maKH))
	BEGIN
		PRINT(N'Khong co khach hang vua nhap. Vui long nhap lai.')
		RETURN
	END
	--Them diem hoat dong
	SET @temp=(SELECT DIEM_HOAT_DONG FROM KHACH_HANG  WITH(XLOCK) WHERE MAKHACHHANG = @maKH)
	WAITFOR DELAY '00:00:05'
	SET @temp=@temp+@diem
	UPDATE KHACH_HANG
	SET DIEM_HOAT_DONG=@temp
	WHERE MAKHACHHANG = @maKH
COMMIT TRAN
GO

--=================================================================================================--
-- TANG DIEM KHI KHACH HANG MUA 1 NHA
CREATE OR ALTER PROCEDURE SP_TANGDIEMKHMUANHA_fix @sdt VARCHAR(10)
AS
BEGIN
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ
DECLARE @temp SMALLINT;		
	IF(NOT EXISTS(SELECT * FROM KHACH_HANG WHERE DIENTHOAIKHACHHANG = @sdt))
	BEGIN
		PRINT(N'Khong co khach hang vua nhap. Vui long nhap lai.')
		RETURN
	END
	--Them diem hoat dong
	SET @temp=(SELECT DIEM_HOAT_DONG FROM KHACH_HANG WHERE DIENTHOAIKHACHHANG = @sdt)
	SET @temp=@temp+100
	UPDATE KHACH_HANG
	SET DIEM_HOAT_DONG=@temp
	WHERE MAKHACHHANG = @sdt
END;
GO	

--=================================================================================================--
--THONG KE KHACH HANG TIEM NANG
CREATE OR ALTER PROC SP_THONGKEKHTIEMNANG_fix @SoLuongL1 INT OUTPUT   ,@SoLuongL2 INT OUTPUT 
AS
BEGIN TRAN
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	SELECT MAKHACHHANG,DIEM_HOAT_DONG FROM KHACH_HANG WHERE DIEM_HOAT_DONG >=700
	SET @SoLuongL1=(SELECT COUNT(*)AS SoLuongL1  FROM KHACH_HANG WHERE DIEM_HOAT_DONG >=700)
	 WAITFOR DELAY'00:00:05'
	 SET @SoLuongL2=(SELECT COUNT(*)AS SoLuongL2 FROM KHACH_HANG WHERE DIEM_HOAT_DONG >=700)
COMMIT TRAN
go

--=================================================================================================--
--CAP NHAT SUA TEN KHACH HANG
CREATE OR ALTER PROC SP_UPDATENAMEKH_fix @maKH VARCHAR(10), @name VARCHAR(50)
as
BEGIN TRAN
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
DECLARE @temp VARCHAR(50)
IF(NOT EXISTS(SELECT * FROM KHACH_HANG WHERE MAKHACHHANG = @maKH))

	BEGIN
		PRINT(N'Khong co khach hang vua nhap. Vui long nhap lai.')
		RETURN
	END
	SET @temp=(SELECT TENKHACHHANG FROM KHACH_HANG WITH(XLOCK) WHERE MAKHACHHANG=@maKH)
	WAITFOR DELAY '00:00:05'
	 SET @temp = @name
UPDATE KHACH_HANG
	SET TENKHACHHANG=@temp
	WHERE MAKHACHHANG = @maKH
COMMIT TRAN

