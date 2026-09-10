using AlejandgoShop.Domain.Common;
using Shouldly;
using Xunit;

namespace AlejandgoShop.UnitTests.Common;

public class EntityTests
{
    private sealed class DummyEntity : Entity { }

    [Fact]
    public void Two_entities_with_different_ids_are_not_equal()
    {
        var a = new DummyEntity();
        var b = new DummyEntity();

        a.Equals(b).ShouldBeFalse();
    }
}