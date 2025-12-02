mergeInto(LibraryManager.library, {
    // 启用阻止浏览器快捷键
    DisableBrowserShortcuts: function() {
        // 检查是否应该阻止（当Unity Canvas存在且焦点不在输入框时）
        function shouldBlockBrowserShortcut() {
            var canvas = document.querySelector('canvas');
            
            // 如果canvas不存在，说明游戏未加载，不阻止
            if (!canvas) {
                return false;
            }
            
            // 获取当前焦点元素
            var activeElement = document.activeElement;
            var tagName = activeElement ? activeElement.tagName.toLowerCase() : '';
            
            // 如果焦点在输入框或文本域，不阻止（让用户可以输入）
            if (tagName === 'input' || tagName === 'textarea') {
                return false;
            }
            
            // 如果焦点在Unity相关元素上，阻止
            if (activeElement === canvas || activeElement === document.body) {
                return true;
            }
            
            // 检查Unity容器
            var unityContainer = document.querySelector('#unity-container');
            if (unityContainer && unityContainer.contains(activeElement)) {
                return true;
            }
            
            // 默认：如果canvas存在且焦点不在输入框，则阻止（游戏正在运行）
            return true;
        }
        
        // 阻止常见的浏览器快捷键
        window.addEventListener('keydown', function(e) {
            // F5 (刷新)
            if (e.key === 'F5' || (e.keyCode === 116)) {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+R (刷新)
            if ((e.ctrlKey || e.metaKey) && e.key === 'r') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+Shift+R (强制刷新)
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'R') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+F5 (强制刷新)
            if ((e.ctrlKey || e.metaKey) && (e.key === 'F5' || e.keyCode === 116)) {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+W (关闭标签) - 仅在游戏窗口获得焦点时阻止，防止意外关闭
            // 只阻止默认行为（关闭标签页），但让事件继续传播到Unity，使Unity可以接收Ctrl+W用于爬行
            if ((e.ctrlKey || e.metaKey) && e.key === 'w' && shouldBlockBrowserShortcut()) {
                e.preventDefault();
                // 不调用 stopPropagation() 或 return false，让事件继续传播到Unity
                // 这样Unity可以接收到Ctrl+W事件用于游戏控制
            }
            
            // Ctrl+N (新窗口)
            if ((e.ctrlKey || e.metaKey) && e.key === 'n') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+T (新标签)
            if ((e.ctrlKey || e.metaKey) && e.key === 't') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+Shift+N (无痕模式) - Chrome
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'N') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // F12 (开发者工具)
            if (e.key === 'F12' || e.keyCode === 123) {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+Shift+I (开发者工具)
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'I') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+Shift+J (开发者工具 - 控制台)
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'J') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+U (查看源代码)
            if ((e.ctrlKey || e.metaKey) && e.key === 'u') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+Shift+Delete (清除浏览数据)
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'Delete') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+H (历史记录)
            if ((e.ctrlKey || e.metaKey) && e.key === 'h') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+J (下载)
            if ((e.ctrlKey || e.metaKey) && e.key === 'j') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+K (搜索栏)
            if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+L (地址栏)
            if ((e.ctrlKey || e.metaKey) && e.key === 'l') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+Shift+P (打印/命令面板)
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'P') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+P (打印)
            if ((e.ctrlKey || e.metaKey) && e.key === 'p') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Alt+Left (后退)
            if (e.altKey && e.key === 'ArrowLeft') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Alt+Right (前进)
            if (e.altKey && e.key === 'ArrowRight') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+S (保存页面) - 仅在游戏窗口获得焦点时阻止
            // 注意：游戏使用Ctrl+S进行爬行，所以需要在游戏运行时阻止浏览器的默认行为
            if ((e.ctrlKey || e.metaKey) && e.key === 's' && shouldBlockBrowserShortcut()) {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+Shift+S (另存为)
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'S') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+O (打开文件)
            if ((e.ctrlKey || e.metaKey) && e.key === 'o') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+Shift+O (书签管理器)
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'O') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+G / F3 (查找下一个)
            if (((e.ctrlKey || e.metaKey) && e.key === 'g') || e.key === 'F3') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+Shift+G / Shift+F3 (查找上一个)
            if (((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'G') || (e.shiftKey && e.key === 'F3')) {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+F (查找) - 仅在游戏窗口获得焦点时阻止，且不在输入框内
            if ((e.ctrlKey || e.metaKey) && e.key === 'f' && shouldBlockBrowserShortcut()) {
                var target = e.target || e.srcElement;
                var tagName = target.tagName ? target.tagName.toLowerCase() : '';
                if (tagName !== 'input' && tagName !== 'textarea') {
                    e.preventDefault();
                    e.stopPropagation();
                    return false;
                }
            }
            
            // Ctrl+B (书签栏)
            if ((e.ctrlKey || e.metaKey) && e.key === 'b') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+D (添加书签) - 仅在游戏窗口获得焦点时阻止
            // 注意：游戏使用Ctrl+D进行爬行，所以需要在游戏运行时阻止浏览器的默认行为
            if ((e.ctrlKey || e.metaKey) && e.key === 'd' && shouldBlockBrowserShortcut()) {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+Shift+B (显示/隐藏书签栏)
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'B') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+, (Chrome设置)
            if ((e.ctrlKey || e.metaKey) && e.key === ',') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
            // Ctrl+. (Chrome设置)
            if ((e.ctrlKey || e.metaKey) && e.key === '.') {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            
        }, true); // 使用捕获阶段，确保优先处理
        
        // 也阻止右键菜单中的某些选项
        window.addEventListener('contextmenu', function(e) {
            // 可选：如果你想完全禁用右键菜单，取消下面的注释
            // e.preventDefault();
            // return false;
        }, true);
        
        // 添加 beforeunload 事件，进一步防止意外关闭（作为额外保护）
        window.addEventListener('beforeunload', function(e) {
            // 这个事件会在用户尝试关闭标签/窗口时触发
            // 但现代浏览器可能会忽略自定义消息
            // 这只是一个额外的保护层
        });
        
        console.log('[WebGL] 浏览器快捷键已禁用');
    },
    
    // 恢复浏览器快捷键（如果需要的话）
    EnableBrowserShortcuts: function() {
        // 注意：由于我们使用了匿名函数，无法直接移除事件监听器
        // 如果需要恢复功能，需要保存引用
        console.log('[WebGL] 浏览器快捷键已启用（注意：此功能需要重新加载页面）');
    }
});
