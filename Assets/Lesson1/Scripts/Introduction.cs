using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Introduction : MonoBehaviour
{
    // Ở phần này chúng ta sẽ tìm hiểu về Pooling Object (khái niệm, tại sao nên dùng)
    // Cách tạo và hủy obj thông thường so với Pool, khi nào áp dụng

    /* 
    1. Vấn đề thường thấy.
    - Thông thường khi mới học đến game, khi muốn tạo ra một object chúng ta sẽ dùng hàm Instantiate(object), hàm này sẽ khởi cấp phát bộ nhớ, khởi tạo các thành phần
    - Sau đó nếu không muốn sử dụng object nữa thì sẽ dùng hàm Destroy(object) để xóa obj
    - Khi Destroy(), GameObject được đánh dấu là đã hủy nhưng các object liên kết (script, component, reference)
    vẫn còn tồn tại trong bộ nhớ cho đến khi không còn tham chiếu mới được GC (Garbage Collector) dọn
    - Nếu phải Destroy nhiều object -> tạo nhiều rác -> GC chạy -> giật lag game 
    */

    // Demo đoạn 1
    InstantiateAndDestroy instantiateAndDestroy;

    /* 
    2. Cách giải quyết: sử dụng Pooling Object.
    - Pooling Object là kĩ thuật tối ưu hiệu năng trong game bằng việc tái sử dụng các object đã được tạo sẳn thay vì Instantiate và Destroy liên tục.
    - Object sẽ được active khi cần sử dụng, khi không dùng nữa sẽ deactive
    - Dưới gốc nhìn người chơi sẽ không nhận ra điều này, cho hiệu quả tương tự như tạo và xóa nhưng tối ưu hiệu năng hơn 
    */

    /* 
    3. Luồng hoạt động của Pooling Object

                                                        |----------------------------------------------------------------------------------------------------------------↑                                                                                                             
                                                        ↓                                                                                                                |
    [Start] -> [Tạo Obj, SetActive(false)] -> [đưa vào Pool List] -> [Game yêu cầu Obj] -> [Kiểm tra Pool List] -> [có Obj] -> [Lấy trong Pool List sử dụng] -> [không dùng nữa]
                          ↑                                                                          |
                          |--------------------------------------------------------------------------↓ [Không có Obj]           
    */

    /* 
    4. Tại sao nên dùng Pool, lợi ích của Pool
    - Như đã nêu các vấn đề ở trên, việc dùng Pooling Object sẽ tối ưu hiệu năng game, giảm giật lag, tuột FPS
    - Không cần tạo và hủy Object mỗi lần dùng, tiết kiệm CPU
    - Tránh sinh ra rác quản lý bởi C#
    - Chỉ cần tạo Object 1 lần sau đó tái sử dụng vô hạn
    */

    //Demo đoạn 2, 3, 4
    MySimplePool mySimplePool;

    /*
    5. Một số ví dụ
    - Pooling Object là linh hồn của lập trình game, được sử dụng rất nhiều
    - Dùng để spawn viên đạn
    - Spawn Enemy
    - Spawn hiệu ứng
    - Spawn âm thanh
    - Spawn UI,...
    - Hầu như tất cả mọi thứ tái sử dụng đề dùng Pool
    */
}
