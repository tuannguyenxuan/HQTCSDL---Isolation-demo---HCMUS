--======================================Fix lỗi Phantom===========================================================--
--------Khách hàng tìm kiếm danh sách nhà theo loại nhà (mua hay thuê)-----

 create PROC sp_fix_TimKiemNha
@Loainha CHAR(1), @count int out
AS
BEGIN TRAN
	BEGIN TRY
		SET TRANSACTION ISOLATION LEVEL SERIALIZABLE --READ COMMITTED REPEATABLE READ 
  		IF @Loainha = '0' ---Tim nha de thue
		BEGIN
			---Kiểm tra nhà ở tình trạng chưa được thuê thì mới xuất ra
			IF NOT EXISTS (SELECT * FROM [dbo].[NHA_THUE] NT WHERE NT.TINHTRANGNHA = '0')
				BEGIN
					PRINT('Không có nhà đáp ứng yêu cầu của quý khách hàng!!!')
					RAISERROR (N'Không có nhà đáp ứng yêu cầu của quý khách hàngccc!!!', 0, 0)
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

----Demo

GO
--tran1 Khách hàng tìm kiếm nhà 
BEGIN TRAN
	EXEC sp_fix_TimKiemNha '1'
COMMIT TRAN


GO
--tran2 Chủ nhà đăng nhà mới lên 
BEGIN TRAN
	EXEC sp_DangNha 6, 'Ho Chi Minh',N'4','1', 500000000000
COMMIT TRAN


--======================================Fix lỗi Unrepeatable Read===========================================================--
--------Nhân viên xem thông tin khách hàng có Tenkh được truyền vào----- 
CREATE OR ALTER PROC sp_fix_XemThongTinKhachHang
@Tenkh NVARCHAR(20), @out varchar(50) out 
AS
BEGIN TRAN
		SET TRANSACTION ISOLATION LEVEL REPEATABLE READ 
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


----Demo

GO
--tran1 Nhân viên xem thông tin khách hàng khi nhập vào tên
BEGIN TRAN
	---Xem thông tin khách hàng có tên 'Geoffrey Delgado'
	EXEC sp_fix_XemThongTinKhachHang 'Geoffrey Delgado'
COMMIT TRAN


GO
--tran2 Khách hàng chỉnh sửa thông tin tên
BEGIN TRAN
	EXEC sp_ChinhSuaThongTinKH 'KH000', N'Trang', '1', '0388838362'
COMMIT TRAN

UPDATE [dbo].[KHACH_HANG] SET TENKHACHHANG = 'Geoffrey Delgado' WHERE MAKHACHHANG = 'KH000'

--================================Fix Lỗi Deadlock =================================================================--
 -- Nhân viên cập nhật lại tình trạng nhà đã được bán 
CREATE OR ALTER PROC sp_fix_BanNha
@Manha NVARCHAR(10)
AS
BEGIN TRAN
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE 
	IF (NOT EXISTS (SELECT MANHA FROM [dbo].[NHA_BAN] WITH (XLOCK) WHERE MANHA = @Manha AND TINHTRANGNHA = '0') 
	AND NOT EXISTS (SELECT MANHA FROM [dbo].[NHA] WITH (XLOCK) WHERE MANHA = @Manha AND TINHTRANGNHA = '0'))
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
--====================================================================
--tran1 Nhân viên cập nhật lại tình trạng nhà được bán 
BEGIN TRAN
	EXEC sp_fix_BanNha 'N199'
COMMIT TRAN

--====================================================================
--tran2 Chủ nhà sửa thông tin nhà
BEGIN TRAN
	EXEC sp_SuaThongTinNha 'N199', 'HCM', N'Gò Vấp', 12
COMMIT TRAN

SELECT * FROM [dbo].[NHA] WHERE MANHA = 'N199'

SELECT * FROM [dbo].[NHA_BAN] WHERE MANHA = 'N199'
--====================================================================
--Update lại dữ liệu
UPDATE [dbo].[NHA_BAN] SET TINHTRANGNHA = '0' WHERE MANHA = 'N199'
UPDATE [dbo].[NHA] SET TINHTRANGNHA = '0' WHERE MANHA = 'N199'






