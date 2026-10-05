using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;

namespace LongTool.Core.Storage
{
    /// <summary>
    /// Schema lưu metadata của Door Board vào Legend View.
    /// Dùng Extensible Storage để dữ liệu đi theo model.
    /// </summary>
    public static class DoorBoardMetadataSchema
    {
        // ⚠️ KHÔNG đổi GUID này sau khi đã release.
        // Nếu muốn thêm field mới, tạo Schema mới với GUID khác.
        public static readonly Guid SchemaGuid =
            new Guid("98228990-0081-4e03-8cbe-ce78f8f42e43"); // ✅ GUID mới

        public const string SchemaName = "LongTool_DoorBoard";
        public const string FieldOriginX = "OriginX";
        public const string FieldOriginY = "OriginY";
        public const string FieldOriginZ = "OriginZ";
        public const string FieldSymbol = "Symbol";
        public const string FieldCreatedAt = "CreatedAt";
        public const string FieldUpdatedAt = "UpdatedAt";

        /// <summary>
        /// Lấy hoặc tạo Schema. Chỉ tạo 1 lần cho mỗi Document.
        /// </summary>
        public static Schema GetOrCreateSchema()
        {
            Schema? schema = Schema.Lookup(SchemaGuid);

            if (schema != null)
                return schema;

            SchemaBuilder builder =
                new SchemaBuilder(SchemaGuid);

            builder.SetSchemaName(SchemaName);
            builder.SetReadAccessLevel(AccessLevel.Public);
            builder.SetWriteAccessLevel(AccessLevel.Public);

            // ✅ SỬA: Gán Spec cho field kiểu double
            FieldBuilder originXField = builder.AddSimpleField(FieldOriginX, typeof(double));
            originXField.SetSpec(SpecTypeId.Length);   // Quan trọng: quy định đây là đơn vị độ dài (mm, m, ...)

            FieldBuilder originYField = builder.AddSimpleField(FieldOriginY, typeof(double));
            originYField.SetSpec(SpecTypeId.Length);

            FieldBuilder originZField = builder.AddSimpleField(FieldOriginZ, typeof(double));
            originZField.SetSpec(SpecTypeId.Length);

            // Các field string không cần Spec
            builder.AddSimpleField(FieldSymbol, typeof(string));
            builder.AddSimpleField(FieldCreatedAt, typeof(string));
            builder.AddSimpleField(FieldUpdatedAt, typeof(string));

            return builder.Finish();
        }

        /// <summary>
        /// Ghi metadata vào Legend View.
        /// Phải gọi trong Transaction.
        /// </summary>
        public static void Write(
            View legend,
            XYZ origin,
            string symbol,
            string? createdAtOverride = null)
        {
            Schema schema = GetOrCreateSchema();

            Entity entity = new Entity(schema);

            // ✅ SỬA: Ghi double kèm đơn vị feet (đơn vị nội bộ của Revit)
            entity.Set(schema.GetField(FieldOriginX), origin.X, UnitTypeId.Feet);
            entity.Set(schema.GetField(FieldOriginY), origin.Y, UnitTypeId.Feet);
            entity.Set(schema.GetField(FieldOriginZ), origin.Z, UnitTypeId.Feet);

            entity.Set(schema.GetField(FieldSymbol), symbol ?? string.Empty);

            entity.Set(
                schema.GetField(FieldCreatedAt),
                createdAtOverride ?? DateTime.Now.ToString("o"));

            entity.Set(
                schema.GetField(FieldUpdatedAt),
                DateTime.Now.ToString("o"));

            legend.SetEntity(entity);
        }

        /// <summary>
        /// Đọc metadata từ Legend View. Trả về null nếu chưa có.
        /// </summary>
        public static DoorBoardMetadata? Read(View legend)
        {
            Schema? schema = Schema.Lookup(SchemaGuid);
            if (schema == null) return null;

            Entity? entity = legend.GetEntity(schema);
            if (entity == null || !entity.IsValid())
                return null;

            Field? originXField = schema.GetField(FieldOriginX);
            if (originXField == null) return null;

            // ✅ SỬA: Truyền UnitTypeId.Feet khi đọc double
            double x = entity.Get<double>(originXField, UnitTypeId.Feet);
            double y = entity.Get<double>(schema.GetField(FieldOriginY), UnitTypeId.Feet);
            double z = entity.Get<double>(schema.GetField(FieldOriginZ), UnitTypeId.Feet);

            string symbol = entity.Get<string>(schema.GetField(FieldSymbol))
                            ?? string.Empty;

            string createdAt = entity.Get<string>(schema.GetField(FieldCreatedAt))
                               ?? string.Empty;

            string updatedAt = entity.Get<string>(schema.GetField(FieldUpdatedAt))
                               ?? string.Empty;

            return new DoorBoardMetadata
            {
                Origin = new XYZ(x, y, z),
                Symbol = symbol,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };
        }
    }

    /// <summary>
    /// Dữ liệu metadata của Door Board lưu trong Legend.
    /// </summary>
    public class DoorBoardMetadata
    {
        public XYZ Origin { get; set; } = XYZ.Zero;
        public string Symbol { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;
    }
}