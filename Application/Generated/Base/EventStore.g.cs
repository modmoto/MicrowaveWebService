//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Application
{
    using System;
    using Domain;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using Application.Users.Hooks;
    
    
    public class EventStore : IEventStore
    {
        
        public IEventStoreRepository EventStoreRepository { get; }
        
        public List<IDomainHook> DomainHooks { get; private set; } = new List<IDomainHook>();
        
        public EventStore(IEventStoreRepository EventStoreRepository, SendPasswordMailHook SendPasswordMailHook, CheckAgeRequirementHook AgeRequirementHook)
        {
            this.EventStoreRepository = EventStoreRepository;
            DomainHooks.Add(SendPasswordMailHook);
            DomainHooks.Add(AgeRequirementHook);
        }
        
        public async Task<HookResult> AppendAll(List<DomainEventBase> domainEvents)
        {
            var enumerator = domainEvents.GetEnumerator();
            for (
            ; enumerator.MoveNext(); 
            )
            {
                var domainEvent = enumerator.Current;
                var domainHooks = DomainHooks.Where(hook => hook.EventType == domainEvent.GetType());
                var enumeratorHook = domainHooks.GetEnumerator();
                for (
                ; enumeratorHook.MoveNext(); 
                )
                {
                    var domainHook = enumeratorHook.Current;
                    var validationResult = await domainHook.ExecuteSavely(domainEvent);
                    if (!validationResult.Ok)
                    {
                        return validationResult;
                    }
                }
            }
            await EventStoreRepository.AddEvents(domainEvents);
            return HookResult.OkResult();
        }
    }
}
