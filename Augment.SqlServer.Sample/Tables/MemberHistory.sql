create table dbo.MemberHistory
(
	member_id			int not null,
	last_seen			datetime not null
)
go

alter table dbo.MemberHistory
	add constraint PK_MemberHistory
	primary key (member_id, last_seen)
go

alter table dbo.MemberHistory
	add constraint FK_MemberHistory_Member
	foreign key (member_id)
	references dbo.Member (member_id)
	on delete cascade on update cascade
go