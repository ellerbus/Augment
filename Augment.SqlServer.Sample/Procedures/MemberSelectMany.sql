create procedure dbo.MemberSelectMany
as

	select	*
	from	dbo.Member
	order by member_id desc

go