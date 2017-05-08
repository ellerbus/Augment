--
--	LIST ALL OBJECTS AFFECTED BY CHANGES TO A PARENT OBJECT
--
select	distinct e.referenced_schema_name + '.' + e.referenced_entity_name				entity,
		object_schema_name(e.referencing_id) + '.' + object_name(e.referencing_id)		impacts
from	sys.sql_expression_dependencies e
where	e.referenced_id != e.referencing_id
union
select	object_schema_name(k.parent_object_id) + '.' + object_name(k.parent_object_id),
		object_schema_name(k.object_id) + '.' + object_name(k.parent_object_id) + '.' + object_name(k.object_id)
from	sys.key_constraints k
union
select	object_schema_name(k.parent_object_id) + '.' + object_name(k.parent_object_id),
		object_schema_name(k.object_id) + '.' + object_name(k.parent_object_id) + '.' + object_name(k.object_id)
from	sys.foreign_keys k
union
select	object_schema_name(k.referenced_object_id) + '.' + object_name(k.referenced_object_id),
		object_schema_name(k.object_id) + '.' + object_name(k.parent_object_id) + '.' + object_name(k.object_id)
from	sys.foreign_keys k
union
select	object_schema_name(t.parent_id) + '.' + object_name(t.parent_id),
		object_schema_name(t.object_id) + '.' + object_name(t.object_id)
from	sys.triggers t
union
select	schema_name(t.schema_id) + '.' + t.name,
		object_schema_name(c.object_id) +'.' + object_name(c.object_id)
from	sys.types t
		inner join sys.columns c
			on	t.system_type_id = c.system_type_id
			and	t.user_type_id = c.user_type_id
where	is_user_defined = 1
  and	is_table_type = 0
  and	not exists
		(
			select	1
			from	sys.table_types tt
			where	c.object_id = tt.type_table_object_id
		)
union
select	schema_name(t.schema_id) + '.' + t.name,
		schema_name(tt.schema_id) + '.' + tt.name
from	sys.types t
		inner join sys.columns c
			on	t.system_type_id = c.system_type_id
			and	t.user_type_id = c.user_type_id
		inner join sys.table_types tt
			on	c.object_id = tt.type_table_object_id
where	t.is_user_defined = 1
--  and	is_table_type = 0
--  and	not exists
--		(
--			select	1
--			from	sys.table_types tt
--			where	c.object_id = tt.type_table_object_id
--		)
