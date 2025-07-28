using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Introduction : MonoBehaviour
{
    // Ở phần này chúng ta sẽ tìm hiểu về Pooling Object (khái niệm, tại sao nên dùng)
    // Cách tạo và hủy obj thông thường so với Pool, khi nào áp dụng

    // 1. Vấn đề thường thấy.
    // - Thông thường khi mới học đến game, khi muốn tạo ra một object chúng ta sẽ dùng
    // hàm Instantiate(object), hàm này sẽ khởi cấp phát bộ nhớ, khởi tạo các thành phần
    // - Sau đó nếu không muốn sử dụng object nữa thì sẽ dùng hàm Destroy(object) để xóa obj
    // - Khi Destroy(), GameObject được đánh dấu là đã hủy nhưng các object liên kết (script, component, reference)
    // vẫn còn tồn tại trong bộ nhớ cho đến khi không còn tham chiếu mới được GC (Garbage Collector) dọn
    // - Nếu phải Destroy nhiều object -> tạo nhiều rác -> GC chạy -> giật lag game

    // Demo đoạn 1
    InstantiateAndDestroy instantiateAndDestroy;

}
