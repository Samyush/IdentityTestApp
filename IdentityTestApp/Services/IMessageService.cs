﻿namespace IdentityTestApp.Services;

public interface IMessageService
{
    Task Send(string email, string subject, string message);
}