using System;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace ThreadTapAssistant.Helpers
{
    public class ApplyResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public static class SwSketchHelper
    {
        /// <summary>
        /// 将指定尺寸（以毫米为单位）应用到当前 SolidWorks 活动模型中选定的对象（尺寸标注或草图圆）
        /// </summary>
        /// <param name="swApp">SolidWorks Application 对象</param>
        /// <param name="targetDiameterMm">底孔目标直径（毫米）</param>
        /// <param name="isRadiusMode">当前尺寸是否为半径（若为半径，实际写入 targetDiameterMm / 2）</param>
        /// <returns>操作结果与提示信息</returns>
        public static ApplyResult ApplySizeToSelection(ISldWorks swApp, double targetDiameterMm, bool isRadiusMode)
        {
            if (swApp == null)
            {
                return new ApplyResult { Success = false, Message = "SolidWorks 应用程序接口未初始化！" };
            }

            var model = swApp.ActiveDoc as IModelDoc2;
            if (model == null)
            {
                return new ApplyResult { Success = false, Message = "当前没有打开或激活的零件/装配体文档，请先打开模型！" };
            }

            var selMgr = model.SelectionManager as ISelectionMgr;
            if (selMgr == null)
            {
                return new ApplyResult { Success = false, Message = "无法获取 SolidWorks 选择管理器！" };
            }

            int selCount = selMgr.GetSelectedObjectCount2(-1);
            if (selCount == 0)
            {
                return new ApplyResult
                {
                    Success = false,
                    Message = "请先在草图中鼠标点击选中【要修改的尺寸标注】或【草图圆】，然后再点击此按钮！"
                };
            }

            // 换算为 SolidWorks 底层统一系统单位：米 (Meter)
            double targetValueInMm = isRadiusMode ? (targetDiameterMm / 2.0) : targetDiameterMm;
            double targetValueInMeters = targetValueInMm / 1000.0;

            bool modifiedAny = false;
            string detailMessage = "";

            for (int i = 1; i <= selCount; i++)
            {
                int objType = selMgr.GetSelectedObjectType3(i, -1);

                // 1. 选中的是尺寸标注 (DisplayDimension)
                if (objType == (int)swSelectType_e.swSelDIMENSIONS)
                {
                    var dispDim = selMgr.GetSelectedObject6(i, -1) as IDisplayDimension;
                    if (dispDim != null)
                    {
                        var dim = dispDim.GetDimension2(0);
                        if (dim != null)
                        {
                            dim.SystemValue = targetValueInMeters;
                            modifiedAny = true;
                            detailMessage = string.Format("已成功将尺寸【{0}】修改为 {1:0.###} mm！", dim.FullName, targetValueInMm);
                            break;
                        }
                    }
                }
                // 2. 选中的是草图弧/圆 (SketchArc / Circle)
                else if (objType == (int)swSelectType_e.swSelSKETCHSEGS)
                {
                    var skSeg = selMgr.GetSelectedObject6(i, -1) as ISketchSegment;
                    if (skSeg != null && skSeg.GetType() == (int)swSketchSegments_e.swSketchARC)
                    {
                        var skArc = skSeg as ISketchArc;
                        if (skArc != null)
                        {
                            // 设定草图圆半径（米）
                            double radiusInMeters = (targetDiameterMm / 2.0) / 1000.0;
                            skArc.SetRadius(radiusInMeters);
                            modifiedAny = true;
                            detailMessage = string.Format("已成功将选中的草图圆直径修改为 {0:0.###} mm (半径 {1:0.###} mm)！", targetDiameterMm, targetDiameterMm / 2.0);
                            break;
                        }
                    }
                    else if (skSeg != null && skSeg.GetType() == (int)swSketchSegments_e.swSketchLINE)
                    {
                        return new ApplyResult
                        {
                            Success = false,
                            Message = "检测到您选中了旋转切除的草图线段。\n提示：请直接用鼠标选中该线与中心线之间的【旋转直径尺寸数字】，再次点击应用即可自动修改！"
                        };
                    }
                }
            }

            if (modifiedAny)
            {
                // 重建模型并刷新图形窗口
                model.EditRebuild3();
                return new ApplyResult { Success = true, Message = detailMessage };
            }

            return new ApplyResult
            {
                Success = false,
                Message = "未能识别当前选中对象的尺寸属性。\n建议：请在草图中直接点选【尺寸数值】或【圆】，或者使用【复制数值】按钮后手动粘贴。"
            };
        }
    }
}
