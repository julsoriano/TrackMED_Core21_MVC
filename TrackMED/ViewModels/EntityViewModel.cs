using System.Collections;
using System.Collections.Generic;
using TrackMED.Models;

namespace TrackMED.ViewModels
{
    public class EntityViewModel<T> where T :  IEntity
    {
        public IEnumerable<T> EntityItems { get; set; }
        public IEntity EntityType {get; set;}

    // The default constructor has no parameters. The default constructor  
    // is invoked in the processing of object initializers.  
    // You can test this by changing the access modifier from public to  
    // private. The declarations in Main that use object initializers will  
    // fail. See http://msdn.microsoft.com/en-us/library/vstudio/bb397680.aspx
        public EntityViewModel() { }

        public EntityViewModel(IEnumerable<T> items, T item)
        {
            EntityItems = items;
            EntityType = item;
        }
    }
}