﻿using System;
using System.Collections.Generic;

namespace Domain.Users
{
    public partial class User
    {
        public ValidationResult UpdateAge(int Age)
        {
            this.Age = Age;
            return ValidationResult.OkResult(new List<DomainEventBase>{new UserUpdateAgeEvent(this.Age, this.Id)});
        }

        public ValidationResult UpdateName(string Name)
        {
            if (Name.Length < 20) { 
                this.Name = Name;
                return ValidationResult.OkResult(new List<DomainEventBase> { new UserUpdateNameEvent(this.Name, this.Id) });
            }
            return ValidationResult.ErrorResult(new List<string>{ "Name is too long"});
        }

        public static CreationResult<User> Create(string name)
        {
            if (name.Length > 4) {
                var newGuid = Guid.NewGuid();
                var user = new User(newGuid, name);
                return CreationResult<User>.OkResult(new List<DomainEventBase> {new CreateUserEvent(user, newGuid)}, user);
            }

            return CreationResult<User>.ErrorResult(new List<string> {"Name too short"});
        }
    }
}