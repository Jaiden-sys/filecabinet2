using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace filecabinet
{
    public class FileCabinetDefaultSevice : FileCabinetService
    {
        protected override void ValidateParameters(RecordRequest request)
        {
            if (request.FirstName == null) { throw new ArgumentNullException(nameof(request.FirstName), "firstname cannot be null"); }
            if (request.LastName == null) { throw new ArgumentNullException(nameof(request.LastName), "lastname cannot be null"); }
            if (request.FirstName.Length < 2 || request.FirstName.Length > 60 || string.IsNullOrWhiteSpace(request.FirstName)) throw new ArgumentException("Invalid first name", nameof(request.FirstName));

            if (request.LastName.Length < 2 || request.LastName.Length > 60 || string.IsNullOrWhiteSpace(request.LastName)) throw new ArgumentException("Invalid last name", nameof(request.LastName));

            if (request.DateOfBirth == default(DateTime))
                throw new ArgumentException("Date of birth is not specified", nameof(request.DateOfBirth));

            if (request.DateOfBirth < limitedDateOfBirth || request.DateOfBirth > DateTime.Today)
                throw new ArgumentException("Invalid date of birth", nameof(request.DateOfBirth));

            if (request.ArchiveId <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.ArchiveId), "Archive ID must be positive");

            if (request.Weight <= 0 || request.Weight > 200)
                throw new ArgumentOutOfRangeException(nameof(request.Weight), "Weight must be between 0 and 200");

            if (request.Type == ' ')
                throw new ArgumentException("Type cannot be empty/space", nameof(request.Type));
            
        }
    }
}
