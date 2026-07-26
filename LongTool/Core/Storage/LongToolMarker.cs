using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;

namespace LongTool.Core.Storage;

public static class LongToolMarker
{
    private static readonly Guid SchemaGuid =
        new("8F6B18A3-0F90-49E3-8A89-7C24E2A1B4E5");

    private const string FieldName = "LongTool";

    private static Schema GetOrCreateSchema()
    {
        Schema? schema = Schema.Lookup(SchemaGuid);

        if (schema != null)
            return schema;

        SchemaBuilder builder = new(SchemaGuid);

        builder.SetSchemaName("LongTool");

        builder.SetReadAccessLevel(AccessLevel.Public);

        builder.SetWriteAccessLevel(AccessLevel.Public);

        builder.AddSimpleField(FieldName, typeof(bool));

        return builder.Finish();
    }

    public static void Mark(Element element)
    {
        Schema schema = GetOrCreateSchema();

        Entity entity = new(schema);

        entity.Set(FieldName, true);

        element.SetEntity(entity);
    }

    public static bool IsMarked(Element element)
    {
        Schema? schema = Schema.Lookup(SchemaGuid);

        if (schema == null)
            return false;

        Entity entity = element.GetEntity(schema);

        if (!entity.IsValid())
            return false;

        return entity.Get<bool>(FieldName);
    }
}