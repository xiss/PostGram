﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PostGram.DAL.Entities.Configs
{
    internal class UserConfig:IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            //TODO Как правильно сделать отображение ошибок при попытке вставить дубль?
            builder.HasIndex(u => u.Login).IsUnique();
        }
    }
}
