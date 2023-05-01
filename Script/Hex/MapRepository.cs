using Godot;

using System.Collections.Generic;
using System.Runtime;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;

using TribesOfDust.Utils;
using TribesOfDust.Hex;

namespace TribesOfDust.Core
{
    public partial class MapRepository : Repository<string, Map>
    {
        /// <summary>
        /// The default resource path used for maps.
        /// </summary>
        private static readonly string DefaultPath = "res://Assets/Maps";
        protected override List<Map> LoadAll() => LoadAll(DefaultPath);

        protected override bool TryLoad(string resourcePath, out Map? asset)
        {
			asset = null;
			
			var targetFile = FileAccess.Open(resourcePath, FileAccess.ModeFlags.Read);
			var fileOpenError = targetFile.GetError();

			// If opening the file worked, deserialize the template map.
			if (fileOpenError == Godot.Error.Ok)
			{
				using var reader = new System.IO.StringReader(targetFile.GetAsText());
				using var xml = XmlReader.Create(reader);

				var deserializer = new DataContractSerializer(typeof(Map));
				asset = deserializer.ReadObject(xml) as Map;
			}

            return asset is not null;
        }

        /// <summary>
        /// Tries to save the map at its' default location.
        /// </summary>
        /// 
        /// <param name="asset">The map to save.</param>
        /// <returns>True, if the map was saved succesfully, false otherwise.</returns>
        public bool TrySave(Map asset)
        {
            var targetPath = DefaultPath + "/"  + asset.Name.ToLower().Replace(' ', '_') + ".xml";
	        var targetFile = FileAccess.Open(targetPath, FileAccess.ModeFlags.Write);
	        
			var fileOpenError = targetFile.GetError();

			// If opening the file worked, serialize the template map and store it in the file as JSON.
			if (fileOpenError == Godot.Error.Ok)
			{
				var settings = new XmlWriterSettings
				{
					Indent = true,
					NewLineHandling = NewLineHandling.Entitize
				};

				var str = new StringBuilder();
				using var xml = XmlWriter.Create(str, settings);

				var serializer = new DataContractSerializer(typeof(Map));
				serializer.WriteObject(xml, asset);
				xml.Flush();

				targetFile.StoreLine(str.ToString());
				targetFile.Close();

                // Add the map to the repository if it isn't already in there.
                // If the map is already in the repository, remove it from the
                // repository and add the newly stored variant.

                if (HasVariations(asset.Name))
                    RemoveAll(asset.Name);

                AddVariation(asset);

                return true;
			}
            
            return false;
        }
    }
}