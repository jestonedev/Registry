// Этот файл используется анализом кода для поддержки атрибутов 
// SuppressMessage, примененных в этом проекте. 
// Подавления на уровне проекта либо не имеют целевого объекта, либо для них задан 
// конкретный объект и область пространства имен, тип, член и т. д.
//
// Чтобы добавить подавление к этому файлу, щелкните правой кнопкой 
// мыши сообщение в списке ошибок, укажите на команду "Подавить сообщения" и выберите вариант 
// "В файле проекта для блокируемых предупреждений".
// Нет необходимости вручную добавлять подавления к этому файлу.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Проверка запросов SQL на уязвимости безопасности", Scope = "member", Target = "Security.AccessControl.#LoadPriveleges()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Scope = "type", Target = "Security.AccessControl")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags", Scope = "type", Target = "Security.Priveleges")]
