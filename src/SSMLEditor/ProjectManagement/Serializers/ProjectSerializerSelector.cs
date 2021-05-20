namespace SSMLEditor.ProjectManagement
{
    using Catel;
    using Catel.IoC;
    using Orc.ProjectManagement;

    internal class ProjectSerializerSelector : IProjectSerializerSelector
    {
        private readonly ITypeFactory _typeFactory;

        public ProjectSerializerSelector(ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => typeFactory);

            _typeFactory = typeFactory;
        }

        public IProjectReader GetReader(string location)
        {
            return _typeFactory.CreateInstance<ProjectReader>();
        }

        public IProjectWriter GetWriter(string location)
        {
            return _typeFactory.CreateInstance<ProjectWriter>();
        }
    }
}
