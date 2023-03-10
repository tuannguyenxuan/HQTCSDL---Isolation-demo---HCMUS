USE [QLND]
GO
/****** Object:  StoredProcedure [dbo].[sp_CapNhatSoPhong]    Script Date: 1/12/2021 1:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[sp_CapNhatSoPhong] @manha varchar(10),@sophong int, @error varchar(max) output
as
begin tran 
	SET tran isolation level repeatable read
	if(exists(select * from NHA where MANHA = @manha) and @sophong > 0)
		BEGIN 
			
		    --select *from NHA where MANHA = @manha
			update NHA set SOPHONG=@sophong where MANHA= @manha
			--select *from NHA where MANHA = @manha
		END 
  --      BEGIN CATCH
		--IF(ERROR_NUMBER() = 1205)
		--begin
		--	SET @Error = ERROR_MESSAGE()
		
		--end
		--END CATCH
	else 
		begin
			set @error = 'Co loi trong luc cap nhat so phong'
			raiserror(@error, 0, 0)
			rollback tran
			return
		end
 commit tran
GO
/****** Object:  StoredProcedure [dbo].[sp_ChuyenQuyenQuanLyNha]    Script Date: 1/12/2021 1:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  proc [dbo].[sp_ChuyenQuyenQuanLyNha] @manv1 varchar(10),@manv2 varchar(10),@error varchar(max) output
as
begin tran 
	set tran isolation level REPEATABLE READ
	if(exists(select * from NHA where MANV = @manv1))
	BEGIN 
		select * from NHA 
		WHERE MANV = @manv1
		waitfor delay '00:00:3'
		update NHA set MANV=@manv2 where MANV=@manv1
		--select * from NHA Where MANHA = 'N057' or MANHA = 'N064' or MANHA = 'N125' or MANHA = 'N128' or MANHA = 'N135' 
	END 
	ELSE BEGIN
			set @error = 'Co loi trong luc chuyen quyen quan ly'
			raiserror(@error, 0, 0)
		rollback tran
		return
	END
 commit tran
GO
/****** Object:  StoredProcedure [dbo].[sp_ChuyenQuyenQuanLyNha_fix]    Script Date: 1/12/2021 1:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  proc [dbo].[sp_ChuyenQuyenQuanLyNha_fix] @manv1 varchar(10),@manv2 varchar(10),@error varchar(50) out
as
begin tran 
	set tran isolation level SERIALIZABLE
	if(exists(select * from NHA where MANV = @manv1))
	begin
		select * from NHA WITH(XLOCK) 
		WHERE MANV = @manv1
		waitfor delay '00:00:3'
		update NHA set MANV=@manv2 where MANV=@manv1
		--select * from NHA Where MANHA = 'N057' or MANHA = 'N064' or MANHA = 'N125' or MANHA = 'N128' or MANHA = 'N135' 
	end
	else
	begin
		set @error = concat('Co loi trong luc chuyen quyen quan ly tu ' ,@manv1 ,'sang ', @manv2)
		raiserror('Co loi trong luc chuyen quyen quan ly', 0, 0)
		rollback tran
		return
	end
 commit tran
GO
/****** Object:  StoredProcedure [dbo].[SP_CONG_DIEM_KH]    Script Date: 1/12/2021 1:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Quản lý tiến hành cộng điểm cho khách hàng
CREATE   PROCEDURE [dbo].[SP_CONG_DIEM_KH] @MAKH VARCHAR(10)
AS
BEGIN TRAN
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED
	DECLARE @DIEM SMALLINT
	SELECT @DIEM = DIEM_HOAT_DONG FROM KHACH_HANG WHERE MAKHACHHANG = @MAKH
	SET @DIEM = @DIEM + 50
	UPDATE KHACH_HANG SET DIEM_HOAT_DONG = @DIEM WHERE MAKHACHHANG = @MAKH
COMMIT TRAN
GO
/****** Object:  StoredProcedure [dbo].[SP_LOGIN]    Script Date: 1/12/2021 1:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Khách hàng đăng nhập với tên đăng nhập và mật khẩu
CREATE   PROCEDURE [dbo].[SP_LOGIN] @ID VARCHAR(10),  @PASS VARCHAR(50)
AS
BEGIN TRAN
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	IF EXISTS (SELECT 1 FROM ACCOUNT WITH(NOLOCK) WHERE _USERNAME = @ID AND _PASSWORD = @PASS)
	BEGIN
		PRINT 'DANG NHAP THANH CONG'		
	END
	ELSE
	BEGIN
		PRINT 'DANG NHAP THAT BAI'
	END
COMMIT TRAN
GO
/****** Object:  StoredProcedure [dbo].[sp_TangDiemNV]    Script Date: 1/12/2021 1:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[sp_TangDiemNV] @manv varchar(10), @diem_cong int,@error varchar(50) out
as
begin tran
		SET TRANSACTION ISOLATION LEVEL READ COMMITTED
		if (exists(select * from NHAN_VIEN where MANV = @manv))
		begin
			--t1 
			declare @diem smallint = (select DIEM_THUONG from NHAN_VIEN 
										WHERE MANV = @manv)
			SET @diem = @diem + @diem_cong
			--t2
			waitfor delay '00:00:5'
			--t1		
			update NHAN_VIEN set DIEM_THUONG =@diem  WHERE MANV = @manv
		end
		else if(@diem < 0) 
		begin
			set @error = 'Co loi trong luc tang diem cho nhan vien'
			raiserror('Co loi trong luc tang diem cho nhan vien', 0, 0)
			rollback tran
			return
		end
commit tran
GO
/****** Object:  StoredProcedure [dbo].[sp_TangDiemNV_fix]    Script Date: 1/12/2021 1:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[sp_TangDiemNV_fix] @manv varchar(10), @diem_cong int,@error varchar(50) out
as
begin tran
		SET TRANSACTION ISOLATION LEVEL REPEATABLE READ
		if (exists(select * from NHAN_VIEN where MANV = @manv))
		begin
			--t1 
			declare @diem smallint = (select DIEM_THUONG from NHAN_VIEN  WITH(XLOCK) 
										WHERE MANV = @manv)
			SET @diem = @diem + @diem_cong
			--t2
			waitfor delay '00:00:5'
			--t1		
			update NHAN_VIEN set DIEM_THUONG =@diem  WHERE MANV = @manv
		end
		else if(@diem < 0) 
		begin
			set @error = 'Co loi trong luc tang diem cho nhan vien'
			raiserror('Co loi trong luc tang diem cho nhan vien', 0, 0)
			rollback tran
			return
		end
commit tran
GO
/****** Object:  StoredProcedure [dbo].[SP_THEM_NV]    Script Date: 1/12/2021 1:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Quản lý nhân viên tiến hành thêm mới 1 nhân viên
CREATE PROCEDURE [dbo].[SP_THEM_NV] 
	@MANV NVARCHAR(10),
	@TEN NVARCHAR(50),
	@GIOITINH NVARCHAR(3),
	@LUONG MONEY,
	@USERNAME NVARCHAR(10),
	@PASS NVARCHAR(10)
AS
BEGIN TRAN
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED
	INSERT INTO ACCOUNT VALUES (@USERNAME, @PASS)
	INSERT INTO NHAN_VIEN VALUES (@MANV, 'CNH01', @TEN, @GIOITINH, 'tphcm', '1/1/2000', 123456789, @LUONG, @USERNAME, 0, 0)
	
COMMIT TRAN
GO
/****** Object:  StoredProcedure [dbo].[SP_THONG_KE_LUONG]    Script Date: 1/12/2021 1:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--=========== Demo lỗi PHANTOM READ
-- Admin thống kê tổng lương của nhân viên trong công ty trong vòng 1 tháng
CREATE   PROCEDURE [dbo].[SP_THONG_KE_LUONG] @TONGL1 MONEY OUT, @TONGL2 MONEY OUT
AS
BEGIN TRAN
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED
	SELECT @TONGL1 = SUM(LUONGNV) FROM NHAN_VIEN 
	PRINT 'TONG LUONG CUA NHAN VIEN TRONG 1 THANG LA:'
	WAITFOR DELAY '0:0:05'
	SELECT @TONGL2 = SUM(LUONGNV) FROM NHAN_VIEN 
COMMIT TRAN
GO
/****** Object:  StoredProcedure [dbo].[SP_THONG_KE_LUONG_FIX]    Script Date: 1/12/2021 1:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[SP_THONG_KE_LUONG_FIX] @TONGL1 MONEY OUT, @TONGL2 MONEY OUT
AS
BEGIN TRAN
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	SELECT @TONGL1 = SUM(LUONGNV) FROM NHAN_VIEN 
	PRINT 'TONG LUONG CUA NHAN VIEN TRONG 1 THANG LA:'
	WAITFOR DELAY '0:0:05'
	SELECT @TONGL2 = SUM(LUONGNV) FROM NHAN_VIEN 
COMMIT TRAN
GO
/****** Object:  StoredProcedure [dbo].[sp_ThongKeNVXuatSac]    Script Date: 1/12/2021 1:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROC [dbo].[sp_ThongKeNVXuatSac] @SoLuongL1 INT OUTPUT   ,@SoLuongL2 INT OUTPUT 
AS
BEGIN TRAN

	SET TRANSACTION ISOLATION LEVEL READ COMMITTED

	SELECT MANV,DIEM_THUONG FROM NHAN_VIEN WHERE DIEM_THUONG >= 500

	SET @SoLuongL1 = (SELECT COUNT(*) SoLuongL1 FROM NHAN_VIEN WHERE DIEM_THUONG >= 500)

	WAITFOR DELAY '00:00:07'

	SET @SoLuongL2 = (SELECT COUNT(*) SoLuongL2  FROM NHAN_VIEN WHERE DIEM_THUONG >= 500)
COMMIT TRAN
GO
/****** Object:  StoredProcedure [dbo].[sp_ThongKeNVXuatSac_fix]    Script Date: 1/12/2021 1:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROC [dbo].[sp_ThongKeNVXuatSac_fix] @SoLuongL1 INT OUTPUT   ,@SoLuongL2 INT OUTPUT
AS
BEGIN TRAN

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

	SELECT MANV,DIEM_THUONG FROM NHAN_VIEN WHERE DIEM_THUONG >= 500

	SET @SoLuongL1 = (SELECT COUNT(*) SoLuongL1 FROM NHAN_VIEN WHERE DIEM_THUONG >= 500)

	WAITFOR DELAY '00:00:07'

	SET @SoLuongL2 = (SELECT COUNT(*) SoLuongL2  FROM NHAN_VIEN WHERE DIEM_THUONG >= 500)

COMMIT TRAN
GO
/****** Object:  StoredProcedure [dbo].[SP_UPDATE_ACCOUNT]    Script Date: 1/12/2021 1:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--=========== Demo lỗi DIRTY READ
-- Nhân viên quản lý thay đổi mật khẩu tài khoản khách hàng
CREATE   PROCEDURE [dbo].[SP_UPDATE_ACCOUNT] @ID VARCHAR(10),  @NEWPASS VARCHAR(50)
AS
BEGIN TRAN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	UPDATE ACCOUNT SET _PASSWORD = @NEWPASS WHERE _USERNAME = @ID
	WAITFOR DELAY '0:0:05'
	-- Mật khẩu mới phải là chữ hoặc chữ và số. Không được là chuỗi số
	IF (ISNUMERIC(@NEWPASS) = 1)
	BEGIN
		ROLLBACK
	END
COMMIT TRAN
GO
/****** Object:  StoredProcedure [dbo].[SP_XEM_DS_KHONG_TIEM_NANG]    Script Date: 1/12/2021 1:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




--=========== Demo lỗi UNREPEATED READ
-- Nhân viên xem danh sách các khách hàng không tiềm năng
CREATE   PROCEDURE [dbo].[SP_XEM_DS_KHONG_TIEM_NANG] @SL1 INT OUT, @SL2 INT OUT
AS
BEGIN TRAN
	SET TRANSACTION ISOLATION LEVEL READ COMMITTED
	SELECT @SL1 = COUNT(*) FROM KHACH_HANG WHERE DIEM_HOAT_DONG < 700
	WAITFOR DELAY '0:0:05'
	SELECT @SL2 = COUNT(*) FROM KHACH_HANG WHERE DIEM_HOAT_DONG < 700
COMMIT TRAN
GO

CREATE OR ALTER PROCEDURE CHECK_NV_TRUONG 
	@USERNAME VARCHAR(50),
	@FLAG INT OUT
AS
BEGIN
	SELECT @FLAG = NV.NHAN_VIEN_TRUONG
	FROM NHAN_VIEN NV
	WHERE NV._USERNAME = @USERNAME
END

