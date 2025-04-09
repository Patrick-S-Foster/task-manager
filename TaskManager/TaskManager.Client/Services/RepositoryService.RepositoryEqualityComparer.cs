using TaskManager.Common;

namespace TaskManager.Client.Services;

public partial class RepositoryService
{
    private class RepositoryEqualityComparer : IEqualityComparer<Repository>
    {
        public bool Equals(Repository? x, Repository? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null || x.GetType() != y.GetType())
            {
                return false;
            }

            return x.Url.Equals(y.Url, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(Repository obj) => obj.Url.GetHashCode();
    }
}