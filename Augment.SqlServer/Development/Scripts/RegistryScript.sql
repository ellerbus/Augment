create table dbo.AugmentRegistry
(
	registry_name			varchar(250) not null,
	sql_script				varchar(max) not null,
	action_enum				varchar(20) not null,
	updated_utc				datetime not null
)
go

alter table dbo.AugmentRegistry
	add constraint PK_AugmentRegistry
	primary key(registry_name)
go
