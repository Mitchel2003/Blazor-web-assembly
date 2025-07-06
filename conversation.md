Por qué existen ambos:

ValidatableViewModel
• Añade a ViewModelBase la capacidad de validar un 
Model
 mediante 
ValidationHelper
c:\Users\herre\Documents\Blazor-web-assembly\AppWeb.ViewModels\Core\Validation\ValidationHelper.cs
.
• No sabe nada de guardar, cargar o borrar – solo expone el estado de errores y métodos Validate/ValidateAsync.
ViewModelCrud<TModel,TId>
• Añade a ViewModelBase (otra vez) todo el flujo CRUD: 
LoadAsync
, 
SaveAsync
, 
DeleteAsync
, estados (IsLoading, IsSaving, …) y comandos pre-cableados.
• Define los métodos abstractos 
GetByIdAsync
c:\Users\herre\Documents\Blazor-web-assembly\AppWeb.ViewModels\Features\Users\UpdateUserVM.cs
, 
OnSaveAsync
c:\Users\herre\Documents\Blazor-web-assembly\AppWeb.ViewModels\Features\Users\UpdateUserVM.cs
, 
OnDeleteAsync
 para que los VM concretos conecten su servicio.
• NO hereda de ValidatableViewModel, por eso vuelve a declarar TModel Model, generando duplicidad.
La “inconsistencia” que ves (CreateUserVM hereda de Validatable… y UpdateUserVM de ViewModelCrud) viene de ahí:

• CreateUserVM solo crea: no necesita toda la artillería CRUD, pero sí validación → usa ValidatableViewModel.
• UpdateUserVM sí necesita cargar, actualizar, controlar cambios, etc. → usa ViewModelCrud y vuelve a implementar su validación “a mano” (antes de llamar al servicio) porque Validatable… no está en la jerarquía.

Consecuencias actuales

– Duplicación de propiedades (
Model
, IsLoading, etc.) y lógica de validación repetida en los VM que heredan de ViewModelCrud.
– Dos formas distintas de gestionar comandos (AsyncRelayCommand creados a mano en CreateUserVM vs generados en base en UpdateUserVM).
– Mayor superficie de código y riesgo de divergencia.

Cómo simplificar

Heredar ViewModelCrud de ValidatableViewModel
csharp
public abstract partial class ViewModelCrud<TModel, TId>
        : ValidatableViewModel<TModel>
        where TModel : class, new()
{
    // se elimina la propiedad Model duplicada;
    // el constructor llama al de la base:
    protected ViewModelCrud(IModelFactory factory)
        : base(factory.Create<TModel>())
    { … }
}
Mover a ViewModelCrud la validación automática antes de salvar:
csharp
protected virtual async Task SaveAsync()
{
    if (!await ValidateAsync()) { StatusMessage = "Fix validation errors"; return; }
    …
}
Hacer que CreateUserVM herede también de ViewModelCrud:
csharp
public partial class CreateUserVM
        : ViewModelCrud<CreateUserInput, int>, ICreateUserVM
{ … }
– Sobrescribe solo 
OnSaveAsync
 (crear) y opcionalmente 
OnDeleteAsync
 (no aplica).
– Obtiene gratis IsModified, SaveCommand, etc., borrando bastante código duplicado.

Eliminar lógica duplicada de comandos/estados que ahora vienen del base.
Beneficios

• Un único stack: ViewModelBase → ValidatableViewModel → ViewModelCrud → VM concretos.
• Todos los VM que guardan datos se validan de la misma forma.
• Menos líneas y menos bugs por divergencias.
• Fácil extender con nuevas capacidades (p.ej. paginación) escribiendo solo en la capa intermedia.

En resumen, la “inconsistencia” no es un bug, sino una herencia que quedó a mitad de camino. Integrar ambos conceptos en la misma jerarquía (o usar composición, si prefieres) elimina la redundancia y deja un diseño más limpio y coherente.