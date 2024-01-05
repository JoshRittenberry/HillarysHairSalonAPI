using HillarysHairSalon.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<HillarysHairSalonDbContext>(builder.Configuration["HillarysHairSalonDbConnectionString"]);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Get Endpoints

// 1. Endpoint to get all Appointments
app.MapGet("/api/appointments", (HillarysHairSalonDbContext db) =>
{
    return db.Appointments
        .Include(a => a.Stylist)
        .Include(a => a.Customer)
        .Include(a => a.AppointmentServices)
            .ThenInclude(aserv => aserv.Service)
        .Select(a => new AppointmentDTO
        {
            Id = a.Id,
            StylistId = a.StylistId,
            Stylist = new StylistDTO
            {
                Id = a.Stylist.Id,
                FirstName = a.Stylist.FirstName,
                LastName = a.Stylist.LastName,
                PhoneNumber = a.Stylist.PhoneNumber,
                Email = a.Stylist.Email,
                Password = a.Stylist.Password,
                StartDate = a.Stylist.StartDate,
                EndDate = a.Stylist.EndDate,
                IsActive = a.Stylist.IsActive,
                IsAdmin = a.Stylist.IsAdmin,
                Appointments = null
            },
            CustomerId = a.CustomerId,
            Customer = new CustomerDTO
            {
                Id = a.Customer.Id,
                FirstName = a.Customer.FirstName,
                LastName = a.Customer.LastName,
                PhoneNumber = a.Customer.PhoneNumber,
                Email = a.Customer.Email,
                Appointments = null
            },
            Scheduled = a.Scheduled,
            IsComplete = a.IsComplete,
            IsCanceled = a.IsCanceled,
            AppointmentServices = a.AppointmentServices.Select(aserv => new AppointmentServiceDTO
            {
                Id = aserv.Id,
                AppointmentId = aserv.AppointmentId,
                ServiceId = aserv.ServiceId,
                Service = new ServiceDTO
                {
                    Id = aserv.Service.Id,
                    Name = aserv.Service.Name,
                    Cost = aserv.Service.Cost
                }
            }).ToList(),
        })
        .ToList();
});

// 2. Endpoint to get an Appointment by Id
app.MapGet("/api/appointments/{id}", (HillarysHairSalonDbContext db, int id) =>
{
    var appointment = db.Appointments
        .Include(a => a.Stylist)
        .Include(a => a.Customer)
        .Include(a => a.AppointmentServices)
            .ThenInclude(aserv => aserv.Service)
        .SingleOrDefault(a => a.Id == id);

    return Results.Ok(new AppointmentDTO
    {
        Id = appointment.Id,
        StylistId = appointment.StylistId,
        Stylist = new StylistDTO
        {
            Id = appointment.Stylist.Id,
            FirstName = appointment.Stylist.FirstName,
            LastName = appointment.Stylist.LastName,
            PhoneNumber = appointment.Stylist.PhoneNumber,
            Email = appointment.Stylist.Email,
            Password = appointment.Stylist.Password,
            StartDate = appointment.Stylist.StartDate,
            EndDate = appointment.Stylist.EndDate,
            IsActive = appointment.Stylist.IsActive,
            IsAdmin = appointment.Stylist.IsAdmin,
            Appointments = null
        },
        CustomerId = appointment.CustomerId,
        Customer = new CustomerDTO
        {
            Id = appointment.Customer.Id,
            FirstName = appointment.Customer.FirstName,
            LastName = appointment.Customer.LastName,
            PhoneNumber = appointment.Customer.PhoneNumber,
            Email = appointment.Customer.Email,
            Appointments = null
        },
        Scheduled = appointment.Scheduled,
        IsComplete = appointment.IsComplete,
        IsCanceled = appointment.IsCanceled,
        AppointmentServices = appointment.AppointmentServices.Select(aserv => new AppointmentServiceDTO
        {
            Id = aserv.Id,
            AppointmentId = aserv.AppointmentId,
            ServiceId = aserv.ServiceId,
            Service = new ServiceDTO
            {
                Id = aserv.Service.Id,
                Name = aserv.Service.Name,
                Cost = aserv.Service.Cost
            }
        }).ToList(),
    });
});

// 3. Endpoint to get all Customers
app.MapGet("/api/customers", (HillarysHairSalonDbContext db) =>
{
    return db.Customers
        .Include(c => c.Appointments)
            .ThenInclude(a => a.AppointmentServices)
                .ThenInclude(aserv => aserv.Service)
            .Include(c => c.Appointments)
                .ThenInclude(a => a.Stylist)
        .Select(c => new CustomerDTO
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            PhoneNumber = c.PhoneNumber,
            Email = c.Email,
            Appointments = c.Appointments.Select(a => new AppointmentDTO
            {
                Id = a.Id,
                StylistId = a.StylistId,
                Stylist = new StylistDTO
                {
                    Id = a.Stylist.Id,
                    FirstName = a.Stylist.FirstName,
                    LastName = a.Stylist.LastName,
                    PhoneNumber = a.Stylist.PhoneNumber,
                    Email = a.Stylist.Email,
                    Password = a.Stylist.Password,
                    StartDate = a.Stylist.StartDate,
                    EndDate = a.Stylist.EndDate,
                    IsActive = a.Stylist.IsActive,
                    IsAdmin = a.Stylist.IsAdmin,
                    Appointments = null
                },
                CustomerId = a.CustomerId,
                Customer = null,
                Scheduled = a.Scheduled,
                IsComplete = a.IsComplete,
                IsCanceled = a.IsCanceled,
                AppointmentServices = a.AppointmentServices.Select(aserv => new AppointmentServiceDTO
                {
                    Id = aserv.Id,
                    AppointmentId = aserv.AppointmentId,
                    ServiceId = aserv.ServiceId,
                    Service = new ServiceDTO
                    {
                        Id = aserv.Service.Id,
                        Name = aserv.Service.Name,
                        Cost = aserv.Service.Cost
                    }
                }).ToList(),
            }).ToList()
        })
        .ToList();
});

// 4. Endpoint to get a Customer by Id
// 5. Endpoint to get all Services
// 6. Endpoint to get a Service by Id
// 7. Endpoint to get all Stylists
// 8. Endpoint to get a Stylist by Id

// Post Endpoints

// Put Endpoints

// Delete Endpoints

app.Run();