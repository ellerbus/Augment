create table dbo.Member
(
	member_id			int not null identity(100000, 1),
	first_name			varchar(30),
	last_name			varchar(30),
	full_name			as last_name + ', ' + first_name,
	member_date			datetime default getdate(),
	row_version			timestamp not null
)
go

alter table dbo.Member
	add constraint PK_Member
	primary key (member_id)
go

alter table dbo.Member
	add constraint UQ_Member
	unique (full_name)
go