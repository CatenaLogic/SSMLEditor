﻿namespace SSMLEditor.ProjectManagement
{
    using System;
    using Catel.IoC;
    using Orc.ProjectManagement;

    internal class ProjectSerializerSelector : IProjectSerializerSelector
    {
        private readonly ITypeFactory _typeFactory;

        public ProjectSerializerSelector(ITypeFactory typeFactory)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);

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
