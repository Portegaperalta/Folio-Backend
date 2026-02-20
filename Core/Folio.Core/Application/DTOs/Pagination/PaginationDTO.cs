using System;

namespace Folio.Core.Application.DTOs.Pagination;

public record PaginationDTO(int page = 1, int recordsPerPage = 10)
{
  private const int MaxRecordsPerPage = 40;

  public int Page { get; init; } = Math.Max(1, page);
  public int RecordsPerPage { get; init; } = Math.Clamp(recordsPerPage, 1, MaxRecordsPerPage);
}
