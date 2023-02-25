-- Sửa thông tin khách hàng
create PROCEDURE sp_SuaThongTinKH
	@username VARCHAR(10), @tenKH VARCHAR(50), @diachiKH VARCHAR(50), @dienthoaiKH VARCHAR(50)
AS
BEGIN TRAN
	IF (NOT EXISTS(SELECT * FROM dbo.KHACH_HANG WHERE _USERNAME = @username))
	BEGIN
		PRINT(N'Không tồn tại khách hàng trùng với mã nhập vào')
		RETURN
	END
	
	IF(@tenKH = '')
		SET @tenKH = (SELECT TENKHACHHANG FROM dbo.KHACH_HANG WHERE _USERNAME = @username)			
	IF(@dienthoaiKH = '')
		SET @dienthoaiKH = (SELECT DIENTHOAIKHACHHANG FROM dbo.KHACH_HANG WHERE _USERNAME = @username)			
	IF(@diachiKH = '')
		SET @diachiKH = (SELECT	DIACHIKHACHHANG FROM dbo.KHACH_HANG WHERE _USERNAME = @username)
	
	UPDATE dbo.KHACH_HANG
		SET TENKHACHHANG = @tenKH, DIACHIKHACHHANG = @diachiKH , DIENTHOAIKHACHHANG = @dienthoaiKH 
		WHERE _USERNAME = @username
		WAITFOR DELAY '00:00:05'

	IF((LEN(@dienthoaiKH) <> 10) OR (LEFT(@dienthoaiKH, 1) <> '0'))
	BEGIN
		ROLLBACK TRAN
		RETURN
	END

COMMIT TRAN
GO

--=================================================================================================--
-- Xem thông tin khách hàng
create PROCEDURE sp_XemThongTinKH
	@maKH VARCHAR(10)
AS
BEGIN TRAN
	SET TRAN ISOLATION LEVEL READ UNCOMMITTED
	IF (NOT EXISTS(SELECT * FROM dbo.KHACH_HANG WHERE MAKHACHHANG = @maKH))
	BEGIN
		PRINT(N'Không tồn tại khách hàng trùng với mã nhập vào')
		RETURN
	END

	SELECT * FROM dbo.KHACH_HANG WHERE MAKHACHHANG = @maKH

COMMIT TRAN
GO

-- Xem thông tin khách hàng - fix
create PROCEDURE sp_XemThongTinKH_fix
	@maKH VARCHAR(10)
AS
BEGIN TRAN
	SET TRAN ISOLATION LEVEL READ COMMITTED
	IF (NOT EXISTS(SELECT * FROM dbo.KHACH_HANG WHERE MAKHACHHANG = @maKH))
	BEGIN
		PRINT(N'Không tồn tại khách hàng trùng với mã nhập vào')
		RETURN
	END

	SELECT * FROM dbo.KHACH_HANG WHERE MAKHACHHANG = @maKH

COMMIT TRAN
GO

--=================================================================================================--
-- Sửa thông tin nhà
create PROCEDURE sp_SuaThongTinNha
	@maNha VARCHAR(10), @duong VARCHAR(20), @quan VARCHAR(20), @soPhong INT
AS
BEGIN TRAN
	IF (NOT EXISTS(SELECT * FROM dbo.NHA WHERE MANHA = @maNha))
	BEGIN
		PRINT(N'Không tồn tại nhà trùng với mã nhập vào')
		RETURN
	END
	
	IF(@duong = '')
		SET @duong = (SELECT DUONGNHA FROM dbo.NHA WHERE MANHA = @maNha)			
	IF(@quan = '')
		SET @quan = (SELECT QUANNHA FROM dbo.NHA WHERE MANHA = @maNha)			
	IF(@soPhong = '')
		SET @soPhong = (SELECT SOPHONG FROM dbo.NHA WHERE MANHA = @maNha)
			
			
	UPDATE dbo.NHA
	SET DUONGNHA = @duong, QUANNHA = @quan, SOPHONG = @soPhong 
	WHERE MANHA = @maNha
	WAITFOR DELAY '00:00:05'
	
	UPDATE dbo.NHA_BAN
	SET DUONGNHA = @duong, QUANNHA = @quan, SOPHONG = @soPhong 
	WHERE MANHA = @maNha
	UPDATE dbo.NHA_THUE
	SET DUONGNHA = @duong, QUANNHA = @quan, SOPHONG = @soPhong 
	WHERE MANHA = @maNha

	IF (@soPhong <= 0)
	BEgin
		ROLLBACK TRAN
		RETURN
	end
COMMIT TRAN
GO
declare @i int
exec sp_fix_TimKiemNha '1',@i out
print @i

--=================================================================================================--
-- Xem thông tin nhà
create PROCEDURE sp_XemThongTinNha
	@maNha VARCHAR(10)
AS
BEGIN TRAN
	SET TRAN ISOLATION LEVEL READ UNCOMMITTED
	IF (NOT EXISTS(SELECT * FROM dbo.NHA WHERE MANHA = @maNha))
	BEGIN
		PRINT(N'Không tồn tại nhà trùng với mã nhập vào')
		RETURN
	END
	ELSE	
		SELECT * FROM dbo.NHA WHERE MANHA = @maNha
COMMIT TRAN
GO


-- Xem thông tin nhà
create PROCEDURE sp_XemThongTinNha_fix
	@maNha VARCHAR(10)
AS
BEGIN TRAN
	SET TRAN ISOLATION LEVEL READ COMMITTED
	IF (NOT EXISTS(SELECT * FROM dbo.NHA WHERE MANHA = @maNha))
	BEGIN
		PRINT(N'Không tồn tại nhà trùng với mã nhập vào')
		RETURN
	END
	ELSE	
		SELECT * FROM dbo.NHA WHERE MANHA = @maNha
COMMIT TRAN
GO

--=================================================================================================--
-- Xem lương của tất cả nhân viên thuộc 1 chi nhánh và xem tổng tiền lương phải trả cho số nhân viên đó trong 1 tháng
create PROCEDURE sp_XemLuongVaDiemThuong
	@maChiNhanh VARCHAR(10), @tongL money out, @tongDT int out
AS
BEGIN TRAN
	IF (NOT EXISTS(SELECT * FROM dbo.CHI_NHANH WHERE MACHINHANH = @maChiNhanh))
	BEGIN
		PRINT(N'Không tồn tại chi chánh trùng với mã nhập vào')
		RETURN
	END

	SELECT MANV, TENNV, LUONGNV, DIEM_THUONG FROM dbo.NHAN_VIEN where MACHINHANH = @maChiNhanh

	WAITFOR DELAY '00:00:05'

	SET @tongL = (select SUM(LUONGNV) FROM NHAN_VIEN WHERE MACHINHANH = @maChiNhanh)
	SET @tongDT = (select SUM(DIEM_THUONG) FROM NHAN_VIEN WHERE MACHINHANH = @maChiNhanh)

COMMIT TRAN
GO

create PROCEDURE sp_XemLuongVaDiemThuong_fix
	@maChiNhanh VARCHAR(10), @tongL money out, @tongDT int out
AS
BEGIN TRAN
	SET TRAN ISOLATION LEVEL REPEATABLE READ
	IF (NOT EXISTS(SELECT * FROM dbo.CHI_NHANH WHERE MACHINHANH = @maChiNhanh))
	BEGIN
		PRINT(N'Không tồn tại chi chánh trùng với mã nhập vào')
		RETURN
	END

	SELECT MANV, TENNV, LUONGNV, DIEM_THUONG FROM dbo.NHAN_VIEN where MACHINHANH = @maChiNhanh

	WAITFOR DELAY '00:00:05'

	SET @tongL = (select SUM(LUONGNV) FROM NHAN_VIEN WHERE MACHINHANH = @maChiNhanh)
	SET @tongDT = (select SUM(DIEM_THUONG) FROM NHAN_VIEN WHERE MACHINHANH = @maChiNhanh)
COMMIT TRAN
GO

create PROCEDURE sp_ChuyenChiNhanh
	@maNV VARCHAR(10), @maCN VARCHAR(10)
AS
BEGIN TRAN
	IF (NOT EXISTS(SELECT * FROM dbo.CHI_NHANH WHERE MACHINHANH = @maCN))
	BEGIN
		PRINT(N'Không tồn tại chi nhánh trùng với mã nhập vào')
		RETURN
	END

	UPDATE dbo.NHAN_VIEN
	SET MACHINHANH = @maCN
	WHERE MANV = @maNV

COMMIT TRAN
GO