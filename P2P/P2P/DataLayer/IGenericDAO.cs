using System.Collections.Generic;

public interface IGenericDao<T>
{
    void Initialize();
    List<T> GetAll();
    bool Save(T bankAccount);
    bool Update(T bankAccount);
    bool Delete(int id);
    T? GetById(int id);
}