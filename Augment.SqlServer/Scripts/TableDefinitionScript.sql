--
--	GETS COLUMN DEFINITIONS FOR A GIVEN TABLE{0}
--	TO BE MAPPED TO AN INTERNAL OBJECT
--

select	c.name								column_name,
		isnull(i.is_primary_key, 0)			is_primary_key,
		c.is_computed						is_computed,
		case type_name(c.user_type_id)
			when 'timestamp' then 1
			when 'rowversion' then 1
			else 0
		end									is_timestamp
from	sys.columns c
		left join sys.index_columns ic
			on	c.object_id = ic.object_id
			and	c.column_id = ic.column_id
		left join sys.indexes i
			on	i.is_primary_key = 1
			and	c.object_id = i.object_id
			and	ic.index_id = i.index_id
where	c.object_id = object_id('dbo.Member')
order by c.column_id
