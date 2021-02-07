namespace Framework.Flow
{
    public interface IModelObserver<in T> where T : BaseModel<T>
    {
        void ModelChanged(T model);
    }
}