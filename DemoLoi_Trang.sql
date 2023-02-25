USE [mASter]
GO
/****** Object:  DatabASe [QLND]    Script Date: 12/16/2020 7:56:03 PM ******/
USE [QLND]
GO

--==============================Demo Lỗi Phantom =================================
---Tình huống: Khách hàng đang tìm kiếm nhà theo điều kiện, đặt wait for delay thì chủ nhà đăng nhà mới lên thành công liên quan đến điều kiện 
---mà khách hàng đang đọc ==> Khách hàng đọc lại thì thấy có sự thay đổi thấy thêm 1 dòng mới

--------Khách hàng tìm kiếm danh sách nhà chưa được bán hoặc thuê-----

CREATE OR ALTER PROC sp_TimKiemNha 
@Loainha CHAR(1), @count int out
AS
BEGIN TRAN
	BEGIN TRY
		SET TRANSACTION ISOLATION LEVEL READ COMMITTED
  		IF @Loainha = '0' ---Tim nha de thue
		BEGIN
			---Kiểm tra nhà ở tình trạng chưa được thuê thì mới xuất ra
			IF NOT EXISTS (SELECT * FROM [dbo].[NHA_THUE] NT WHERE NT.TINHTRANGNHA = '0')
				BEGIN
					PRINT('Không có nhà đáp ứng yêu cầu của quý khách hàng!!!')
					RAISERROR (N'Không có nhà đáp ứng yêu cầu của quý khách hàng!!!', 0, 0)
					ROLLBACK TRAN
				END
			ELSE
				BEGIN
					---Đếm số dòng sẽ được xuất ra
					---SELECT COUNT(*) AS Soluongdocduoc FROM [dbo].[NHA_THUE] NB WHERE NB.TINHTRANGNHA = '0'
					set @count = (SELECT COUNT(*) AS Soluongdocduoc FROM [dbo].[NHA_THUE] WHERE TINHTRANGNHA = '0')
					WAITFOR DELAY '00:00:5'	
					SELECT * FROM [dbo].[NHA_THUE] NT WHERE NT.TINHTRANGNHA = '0'
				END
		END
		ELSE ---Tim nha de mua
		BEGIN
		---Kiểm tra nhà ở tình trạng chưa được bán thì mới xuất ra
			IF NOT EXISTS (SELECT * FROM [dbo].[NHA_BAN] NB WHERE NB.TINHTRANGNHA = '0')
				BEGIN
					PRINT('Không có nhà đáp ứng yêu cầu của quý khách hàng!!!')
					RAISERROR (N'Không có nhà đáp ứng yêu cầu của quý khách hàng!!!', 0, 0)
					ROLLBACK TRAN
				END
			ELSE
				BEGIN
					---Đếm số dòng sẽ được xuất ra 
					set @count = (SELECT COUNT(*) AS Soluongdocduoc FROM [dbo].[NHA_BAN] NB WHERE NB.TINHTRANGNHA = '0')
					WAITFOR DELAY '00:00:5'
					SELECT * FROM [dbo].[NHA_BAN] NB WHERE NB.TINHTRANGNHA = '0'			
				END
		END
	END TRY
	BEGIN CATCH
		PRINT('Tìm kiếm nhà không thành công!!!')
		RAISERROR (N'Tìm kiếm nhà không thành công!!!', 0, 0)
		ROLLBACK TRAN
	END CATCH
COMMIT TRAN

GO

----Chủ nhà đăng nhà mới lên hệ thống------
CREATE OR ALTER PROC sp_DangNha 
@Sophong INT, @Duong NVARCHAR(20), @Quan NVARCHAR(20), @Loainha CHAR(1), @Gia MONEY
AS
BEGIN TRAN
	BEGIN TRY
		SET TRANSACTION ISOLATION LEVEL READ COMMITTED
		---Tự động SET Manha tăng lên khi được thêm vào
		DECLARE @Manha NVARCHAR(10)
		DECLARE @temp INT = 0
		SELECT @Manha =MAX(MANHA) FROM [dbo].[NHA]
		SET @temp = LEN(@Manha)
		SET @Manha = CAST(SUBSTRING(@Manha, 2, @temp) AS INT);
		SET @Manha = 'N'+ CAST ((@Manha + 1) AS NVARCHAR(10));

		---Thêm vào bảng nhà trước 
		INSERT [dbo].[NHA] ([MANHA], [MANV], [MALN], [MACHINHANH], [DUONGNHA], [QUANNHA], [TINHTRANGNHA], [SOLUOTXEM], [SOPHONG]) VALUES (@Manha, NULL, NULL, NULL, @Duong, @Quan, N'0', 0, @Sophong)
  		IF @Loainha = '0' ---Dang nha cho thue
		BEGIN
		----nếu nhà cho thuê thì thêm vào bảng Nhà thuê
			INSERT [dbo].[NHA_THUE] ([MANHA], [MANV], [MALN], [MACHINHANH], [DUONGNHA], [QUANNHA], [TINHTRANGNHA], [SOLUOTXEM], [SOPHONG], [GIATHUE1THANG]) 
			VALUES (@Manha, NULL, NULL, NULL, @Duong, @Quan, N'0', 0, @Sophong, @Gia)
		END
		ELSE
		BEGIN
		----nếu nhà bán thì thêm vào bảng Nhà bán
			INSERT [dbo].[NHA_BAN] ([MANHA], [MANV], [MALN], [MACHINHANH], [DUONGNHA], [QUANNHA], [TINHTRANGNHA], [SOLUOTXEM], [SOPHONG], [GIABAN], [DIEUKIEN]) 
			VALUES (@Manha, NULL, NULL, NULL, @Duong, @Quan, N'0', 0, @Sophong, @Gia, N'Enter your text')
		END
	END TRY
	BEGIN CATCH
		PRINT N'Đăng nhà không thành công!!!'
		RAISERROR (N'Đăng nhà không thành công!!!', 0, 0)
		ROLLBACK TRAN
	END CATCH
COMMIT TRAN



GO
--==============================Demo Lỗi Unrepeatable Read =================================
---Tình huống: Nhân viên xem thông tin khách hàng khi nhập vào tên, sau đó đặt wait for delay thì khách hàng UPDATE lai thông tin tên của mình
---==> Không báo lỗi nhưng không đọc ra được tên khách hàng tìm kiếm

--------Nhân viên xem thông tin khách hàng có Tenkh được truyền vào
CREATE OR ALTER PROC sp_XemThongTinKhachHang
@Tenkh NVARCHAR(20), @out varchar(50) out 
AS
BEGIN TRAN
		SET TRANSACTION ISOLATION LEVEL READ COMMITTED
		IF NOT EXISTS (SELECT * FROM [dbo].[KHACH_HANG] WHERE TENKHACHHANG = @Tenkh )
		BEGIN
			PRINT N'Không có tên khách hàng đó trong hệ thống'
			RAISERROR(N'Không có tên khách hàng đó trong hệ thống', 0, 0)
			ROLLBACK TRAN
		END
		ELSE
		BEGIN
			---SELECT COUNT(*) AS SLKHACHHANG FROM [dbo].[KHACH_HANG] WHERE TENKHACHHANG = @Tenkh
			set @out = 'Tim thay khach hang theo yeu cau'
			WAITFOR DELAY '00:00:5'
			SELECT * FROM [dbo].[KHACH_HANG] WHERE TENKHACHHANG = @Tenkh 
		END 
COMMIT TRAN
GO


declare @out varchar(50)
exec sp_XemThongTinKhachHang ' Trang tran',@out out
print @out



----Khách hàng chỉnh sửa thông tin cá nhân------
CREATE OR ALTER PROC sp_ChinhSuaThongTinKH
@Makh NVARCHAR(10), @Tenkh NVARCHAR(20), @Diachi NVARCHAR(50), @SDT NVARCHAR(50)
AS
BEGIN TRAN
	BEGIN TRY
		SET TRANSACTION ISOLATION LEVEL READ COMMITTED
		IF NOT EXISTS(SELECT * FROM [dbo].[KHACH_HANG] WHERE _USERNAME = @Makh)
		BEGIN
			PRINT'Chưa có khách hàng đó trong hệ thống';
			RAISERROR(N'Chưa có khách hàng đó trong hệ thống', 0, 0)
			ROLLBACK TRAN
		END
		ELSE
			BEGIN
			UPDATE [dbo].[KHACH_HANG] SET TENKHACHHANG = @Tenkh, DIENTHOAIKHACHHANG = @SDT, DIACHIKHACHHANG = @Diachi WHERE _USERNAME = @Makh
			END
	END TRY
	BEGIN CATCH
		PRINT'Cập nhật thông tin không thành công!!!';
		RAISERROR (N'Cập nhật thông tin không thành công!!!', 0, 0)
		ROLLBACK TRAN
	END CATCH
COMMIT TRAN

GO


--================================ Lỗi Deadlock Cycle =================================================================--
 -- Nhân viên cập nhật lại tình trạng nhà đã được bán 
CREATE OR ALTER PROC sp_BanNha
@Manha NVARCHAR(10)
AS
BEGIN TRAN
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED
	IF NOT EXISTS (SELECT MANHA FROM [dbo].[NHA_BAN] WHERE MANHA = @Manha AND TINHTRANGNHA = '0') 
	BEGIN
		PRINT N'Ma nha khong dung hoac da duoc ban hoac khong dung de ban!Vui long kiem tra lai'
		RAISERROR(N'Ma nha khong dung hoac da duoc ban hoac khong dung de ban!Vui long kiem tra lai',0, 0)
		ROLLBACK TRAN
	END
	ELSE
	BEGIN	
		UPDATE [dbo].[NHA_BAN] SET TINHTRANGNHA = '1' WHERE MANHA = @Manha
		WAITFOR DELAY '00:00:5'
		UPDATE [dbo].[NHA] SET TINHTRANGNHA = '1' WHERE MANHA = @Manha
	END
COMMIT TRAN

GO
--=================================================================================================--
--=================================================================================================--
-- Sửa thông tin nhà
CREATE OR ALTER PROCEDURE sp_SuaThongTinNha
@maNha NVARCHAR(10), @duong NVARCHAR(20), @quan NVARCHAR(20), @soPhong INT
AS
BEGIN TRAN
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED
	IF NOT EXISTS(SELECT MANHA FROM dbo.NHA WHERE MANHA = @maNha)
	BEGIN
		PRINT(N'Không tồn tại nhà trùng với mã nhập vào')
		RAISERROR(N'Không tồn tại nhà trùng với mã nhập vào',0, 0)
		ROLLBACK TRAN
		RETURN
	END		
	    UPDATE dbo.NHA
		SET DUONGNHA = @duong, QUANNHA = @quan, SOPHONG = @soPhong 
		WHERE MANHA = @maNha
		WAITFOR DELAY '00:00:5'
		UPDATE dbo.NHA_BAN
		SET DUONGNHA = @duong, QUANNHA = @quan, SOPHONG = @soPhong 
		WHERE MANHA = @maNha
COMMIT TRAN
GO

--====================================================================
--tran1 Nhân viên cập nhật lại tình trạng nhà được bán 
BEGIN TRAN
	EXEC sp_BanNha 'N199'
COMMIT TRAN

--====================================================================
--tran2 Chủ nhà sửa thông tin nhà
BEGIN TRAN
	EXEC sp_SuaThongTinNha 'N000', 'HCM', N'Gò Vấp', 12
COMMIT TRAN

SELECT * FROM [dbo].[NHA] WHERE MANHA = 'N199'

SELECT * FROM [dbo].[NHA_BAN] WHERE MANHA = 'N199'
--====================================================================
--Update lại dữ liệu
CREATE PROC sp_Capnhatdl
@Manha NVARCHAR(10)
AS
BEGIN
	UPDATE [dbo].[NHA_BAN] SET TINHTRANGNHA = '0' WHERE MANHA = @Manha
	UPDATE [dbo].[NHA] SET TINHTRANGNHA = '0' WHERE MANHA = @Manha
END
