using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SengokuWarrior
{
    public static class Target
    {
        public static ITargetable currentTarget;
        private static List<ITargetable> currentTargetList;
        private static int SelectedIndex = -1;
        private static TargetIndicator indicator;
        private static GridPosition currentPosition = new GridPosition(0,0,0);
        public static bool isOutside = false;

        public static Transform CurrentTarget
        {
            get
            {
                return currentTarget.GetTransform();
            }
        }
        public static GridPosition CurrentPosition
        {
            get
            {
                return currentPosition;
            }
            set
            {
                currentPosition = value;
            }
        }

        public static TargetIndicator Indicator
        {
            get
            {
                if (indicator == null) indicator = UIManager._instance.CreateTargetIndicator(new GridPosition(), Color.red, false);

                return indicator;
            }
        }
    
        public static void NextTarget()
        {
            if (currentTargetList.Any())
            {
                if (SelectedIndex != -1)
                {
                    if (SelectedIndex == currentTargetList.Count - 1)
                    {
                        SelectedIndex = 0;
                    }
                    else SelectedIndex++;
                }

                else
                {
                    SelectedIndex = currentTargetList.Count - 1;
                }
                SetTarget(currentTargetList[SelectedIndex]);
            }
        }

        public static void PreviousTarget()
        {
            if (currentTargetList.Any())
            {
                if (SelectedIndex != -1)
                {
                    if (SelectedIndex == 0)
                    {
                        SelectedIndex = currentTargetList.Count - 1;
                    }
                    else SelectedIndex--;
                }
                else
                {
                    SelectedIndex = 0;
                }

                SetTarget(currentTargetList[SelectedIndex]);
            }

        
        }

        public static void SetTarget(ITargetable target, bool show = true)
        {
            if (indicator == null) indicator = UIManager._instance.CreateTargetIndicator(target.getGridPosition(),Color.red,true);
            else
            {
                indicator.SetTarget(target.getGridPosition());
                indicator.SetColor(Color.red);
                indicator.SetShow(show);
            }

            currentTarget = target;
            currentPosition = target.getGridPosition();
            CameraFollow.target = target.GetTransform();

            if (Character.GetCharacterAtTile(currentPosition) != null)
            {
                BattleUIContainer.RefreshUnitView(Character.GetCharacterAtTile(currentPosition));
            }

            if (currentTargetList != null && currentTargetList.Contains(target))
            {
                SelectedIndex = currentTargetList.IndexOf(target);
            }
        }


        public static void Move(GridPosition position, Color color, bool show = true)
        {
            if (indicator == null) indicator = UIManager._instance.CreateTargetIndicator(position, color, true);
            indicator.SetTarget(position);
            indicator.SetColor(color);
            indicator.SetShow(show);

            if (Character.GetCharacterAtTile(position) != null)
            {
                BattleUIContainer.RefreshUnitView(Character.GetCharacterAtTile(position));
            }

            currentPosition = position;

        }

        public static void SetTargetList(List<ITargetable> lst)
        {

            currentTargetList = lst;
            if (currentTargetList.Any())
            {
                SetTarget(lst[0]);
                SelectedIndex = 0;
            }

        }
        public static void SetTargetList(List<Character> lst)
        {
            SetTargetList(lst.Cast<ITargetable>().ToList());
        }

        public static void Clear()
        {
            if (indicator)
                indicator.SetShow(false);
        }

        }
}
