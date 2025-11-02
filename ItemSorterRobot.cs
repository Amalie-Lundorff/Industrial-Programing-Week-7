using System;

namespace InventorySystems_Week6.Models;

public class ItemSorterRobot : Robot
{
    /// <summary>
    /// URScript-skabelon med placeholder {0} for ITEM_X (1=a, 2=b, 3=c).
    /// 1 grid = 0,1 m. S er (3,3), item-bokse er (X,1).
    /// </summary>
    public const string UrscriptTemplate = @"
def move_item_to_shipment_box():
  GRID := 0.1

  SBOX_X := 3
  SBOX_Y := 3
  ITEM_X := {0}
  ITEM_Y := 1

  UP_Z := 0.20
  DOWN_Z := 0.10

  RX := 0.0
  RY := -3.1415
  RZ := 0.0

  def moveto(x, y, z):
    p := p[x, y, z, RX, RY, RZ]
    movej(get_inverse_kin(p), a=1.2, v=0.3, t=0, r=0)
  end

  item_x_m := ITEM_X * GRID
  item_y_m := ITEM_Y * GRID
  s_x_m := SBOX_X * GRID
  s_y_m := SBOX_Y * GRID

  # over vare
  moveto(item_x_m, item_y_m, UP_Z)
  # ned (grib antages automatisk)
  moveto(item_x_m, item_y_m, DOWN_Z)
  # op
  moveto(item_x_m, item_y_m, UP_Z)

  # til S, ned og 'slip'
  moveto(s_x_m, s_y_m, UP_Z)
  moveto(s_x_m, s_y_m, DOWN_Z)
  moveto(s_x_m, s_y_m, UP_Z)
end

move_item_to_shipment_box()
";

    public void PickUp(uint inventoryLocationX)
    {
        if (inventoryLocationX < 1 || inventoryLocationX > 3)
            throw new ArgumentOutOfRangeException(nameof(inventoryLocationX),
                "InventoryLocation skal være 1..3 (a,b,c).");

        var program = string.Format(UrscriptTemplate, inventoryLocationX);
        SendUrscript(program);
    }
}