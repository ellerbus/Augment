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

create index Member_name on dbo.Member (full_name)
go

create trigger dbo.MemberAfterInsert on dbo.Member after insert
as

	insert into dbo.MemberHistory (member_id)
	select	i.member_id
	from	inserted i

go

create type dbo.MemberTableType as table
(
	member_id			int not null,
	first_name			varchar(30),
	last_name			varchar(30)
)
go