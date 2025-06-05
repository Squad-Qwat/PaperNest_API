using API.Helpers.Enums;
using API.Models;
using API.Repositories;
using API.Services;
using API.StateMachineAndUtils;
using Microsoft.AspNetCore.Mvc;

namespace API.Helpers.ExtraClass
{
    public interface ReviewState
    {
        string Name { get; }
        void Process(Review request, ReviewStatus result, string reviewerComment);
    }  
}