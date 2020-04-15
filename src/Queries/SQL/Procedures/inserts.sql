if object_id('InsertUser','P') is not null
	drop proc InsertUser
go

create procedure InsertUser
	@hashed_password as varbinary(64),
	@username as varchar(100),
	@first_name as varchar(100),
	@last_name as varchar(100),
	@email as nvarchar(255),
	@secure_stamp as nvarchar(255)
as
begin

	begin try
		begin transaction

		--insert the user in main tabel
		insert into Users(username,hashed_password)
		values (@username, @hashed_password)

		declare @user_id as int = SCOPE_IDENTITY();

		--insert the user details
		insert into UsersDetails(user_id, first_name, last_name, account_creation_date, email, email_confirmed, timestamp)
		values (@user_id, @first_name, @last_name, GETDATE(), @email, 0, @secure_stamp)
		
		declare @default_role_id as int = 0;

		select @default_role_id = R.role_id from Roles as R
		where R.role_name = 'Normal'

		--set the user role to normal
		insert into UsersRoles(user_id,role_id)
		values (@user_id, @default_role_id)
							  
		commit
		
		select 1
		end try
	begin catch	
		rollback;
		throw;
	end  catch

end

 

go

if object_id('InsertPointsDataset', 'P') is not null
	drop procedure InsertPointsDataset
go
create procedure InsertPointsDataset 
	@username as varchar(100),
	@dataset_name as varchar(100),
	@source_name as varchar(100)
as 
begin

	begin try
		begin transaction
			declare @user_id as int = -1,
					@dataset_id as int = -1;
			
			--Get the user id
			select @user_id = U.user_id
			from Users as U where U.username = @username
			
			--Get the status id of 'Pending' status
			select @status_id = DS.status_id
			from DatasetsStatuses as DS where DS.name = 'Pending' 

			--Insert data into the dataset 
			insert into DataSets(user_id, dataset_name, status_id, source_name)
			values (@user_id, @dataset_name, @status_id, @source_name)
			
			select SCOPE_IDENTITY() as ID;
		commit

		select SCOPE_IDENTITY();
	end try
	begin catch
		rollback;
		throw;
	end catch

end


go

if object_id('InsertColorPalette', 'P') is not null
	drop procedure InsertColorPalette
go
create procedure InsertColorPalette
	@username as varchar(100),
	@palette_name as varchar(255),
	@palette_serialization as text
as 
begin
	begin try
		begin transaction
			declare @user_id as int = -1,
					@status_mask as int = -1; 
			
			--Get the user id
			select @user_id = U.user_id
			from Users as U where U.username = @username

			--Get uploaded status id
			select @status_mask = CPS.status_mask
			from ColorPalettesStatuses as CPS 
			where CPS.name = 'Uploaded'

			--Insert data into the color palettes 
			insert into ColorPalettes(palette_name, palette_serialization, user_id, creation_date, status_mask)
			values (@palette_name, @palette_serialization, @user_id, GETDATE(), @status_mask)
			
			select SCOPE_IDENTITY() as ID;
		commit

		select SCOPE_IDENTITY();
	end try
	begin catch
		rollback;
		throw;
	end catch

end




if object_id('InsertGeoserverColorPalette', 'P') is not null
	drop procedure InsertGeoserverColorPalette
go
create procedure InsertGeoserverColorPalette 
	@geoserver_data_set_id as int,
	@palette_id as int
as 
begin

	begin try
		begin transaction

			--Insert the dataset
			insert into GeoserverDataSetsPalettes(geoserver_dataset_id, color_palette_id)
			values (@geoserver_data_set_id, @palette_id)
			
			select SCOPE_IDENTITY() as ID;
		commit

		select SCOPE_IDENTITY();
	end try
	begin catch
		rollback;
		throw;
	end catch

end




if object_id('InsertGeoserverPointsDataset', 'P') is not null
	drop procedure InsertGeoserverPointsDataset
go
create procedure InsertGeoserverPointsDataset 
	@geoserver_api_url as varchar(255),
	@data_set_id as int,
	@default_color_palette_id as int
as 
begin

	begin try
		begin transaction
			 
			--Insert the dataset
			insert into GeoserverDataSets(geoserver_api_url, data_set_id, default_color_palette_id)
			values (@geoserver_api_url, @data_set_id, @default_color_palette_id)
			
		    declare @inserted_id int = SCOPE_IDENTITY();
			
			exec InsertGeoserverColorPalette
				@geoserver_data_set_id = @inserted_id, 
				@palette_id = @default_color_palette_id



			select @inserted_id as ID
		commit

		select SCOPE_IDENTITY();
	end try
	begin catch
		rollback;
		throw;
	end catch

end