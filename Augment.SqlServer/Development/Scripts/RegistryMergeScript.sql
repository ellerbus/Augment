merge dbo.AugmentRegistry as tgt
using
(
	select
		'RegistryName'	registry_name,
		'SqlScript'		sql_script,
		'Action'		action_enum
)
as x
on
(
	tgt.registry_name = x.registry_name
)
when matched then update set
	tgt.sql_script	= x.sql_script,
	tgt.action_enum	= case when x.action_enum = 'Deleted' then 'Deleted' else 'Updated' end,
	tgt.updated_utc	= getutcdate()
when not matched by target then insert
(
	registry_name,
	sql_script,
	action_enum,
	updated_utc
)
values
(
	x.registry_name,
	x.sql_script,
	'Inserted',
	getutcdate()
);