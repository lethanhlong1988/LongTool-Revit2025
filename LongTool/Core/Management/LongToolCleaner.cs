using System;

using Autodesk.Revit.DB;

namespace LongTool.Core.Management;

public static class LongToolCleaner
{
	public static int DeleteAll(Document document)
	{
		if (document is null)
		{
			throw new ArgumentNullException(nameof(document));
		}

		ICollection<ElementId> elementIds = LongToolElementCollector.Collect(document);

		if (elementIds.Count == 0)
		{
			return 0;
		}

		using Transaction transaction = new(document, "Clear LongTool Objects");

		transaction.Start();

		document.Delete(elementIds);

		transaction.Commit();

		return elementIds.Count;
	}
}