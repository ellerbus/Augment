
insert into dbo.Member (first_name, last_name)
	select	'Augment', 'Sql Server'
	where	not exists (select 1 from dbo.Member where first_name = 'Augment')
