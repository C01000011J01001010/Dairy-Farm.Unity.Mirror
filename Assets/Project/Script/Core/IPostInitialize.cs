using System.Collections;

public interface IPostInitialize : IBaseInitializable 
{ 
    IEnumerator PostInitialize(); 
}