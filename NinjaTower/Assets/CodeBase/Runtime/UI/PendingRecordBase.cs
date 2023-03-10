using System;
using System.Collections.Generic;

namespace Carotaa.Code
{
    public abstract class PendingRecordBase
    {
        public enum Code
        {
            Preload,
            Active,
            DeActive,
            Destroy
        }

        public enum PendingStatus
        {
            Out,
            Wait,
            Discard
        }

        public object[] CreateParam;

        public PendingRecordBase(Type pageType, Code operation)
        {
            PageOperation = operation;
            PageType = pageType;
        }

        public Code PageOperation { get; }
        public Type PageType { get; }

        public abstract PageLayer Layer { get; }

        public LinkedListNode<ControllerBase> Node { get; private set; }
        public ControllerBase Controller => Node.Value;

        public void BindRecord(LinkedListNode<ControllerBase> node)
        {
            Node = node;
        }

        public override string ToString()
        {
            return $"Record: {PageOperation} of Page {PageType}";
        }

        public abstract PendingStatus Tick(UIManager.State state);
    }

    public class PendingSlide : PendingRecordBase
    {
        public PendingSlide(Type pageType, Code operation) : base(pageType, operation)
        {
        }

        public override PageLayer Layer => PageLayer.Slide;

        public override PendingStatus Tick(UIManager.State state)
        {
            return PendingStatus.Out;
        }
    }

    public class PendingPop : PendingRecordBase
    {
        public PendingPop(Type pageType, Code operation) : base(pageType, operation)
        {
        }

        public override PageLayer Layer => PageLayer.Pop;


        public override PendingStatus Tick(UIManager.State state)
        {
            if (PageOperation == Code.Active) return state.PopCount <= 0 ? PendingStatus.Wait : PendingStatus.Out;

            return PendingStatus.Out;
        }
    }

    public class PendingToast : PendingRecordBase
    {
        public PendingToast(Type pageType, Code operation) : base(pageType, operation)
        {
        }

        public override PageLayer Layer => PageLayer.Toast;


        public override PendingStatus Tick(UIManager.State state)
        {
            return PendingStatus.Out;
        }
    }
}