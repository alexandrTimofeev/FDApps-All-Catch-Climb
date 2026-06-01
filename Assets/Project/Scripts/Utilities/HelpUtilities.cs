using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class HelpUtilities
{
    public static Fish ToFish(this FishSO fishSO, float _weight, float _sizeMult)
    {
        Fish fish = new Fish
        {
            nameString = fishSO.nameString,
            strength = fishSO.strength,
            speed = fishSO.speed,
            rarity = fishSO.rarity,
            weight = _weight,
            sizeMult = _sizeMult,
            sellValue = (int)(fishSO.sellValue * _sizeMult),
            fishModel = fishSO.fishModel,
            fishMaterial = fishSO.fishMaterial,
            width = fishSO.width * _sizeMult
        };

        return fish;
    }
    public class Player
    {
        public static Vector3 CalculateTargetLocalPositionStack(FishItem newFish, Stack<FishItem> fishStack)
        {
            float totalHeight = 0f;

            foreach (var fish in fishStack)
            {
                totalHeight += fish.fish.width;
            }

            totalHeight += newFish.fish.width / 2f;

            return Vector3.up * totalHeight;
        }

        public static void PickupFishAndPutToStack(FishItem fishItem, Transform inputFishStackPos, Stack<FishItem> pickedUpFish, Action onComplete = null)
        {
            Vector3 startWorldPos = fishItem.transform.position;

            fishItem.transform.SetParent(inputFishStackPos, worldPositionStays: true);

            Vector3 targetLocalPos = CalculateTargetLocalPositionStack(fishItem, pickedUpFish);
            Vector3 targetWorldPos = inputFishStackPos.TransformPoint(targetLocalPos);

            float peakHeight = 3f;
            float animationLength = 0.4f;

            Vector3 midWorldPos = (startWorldPos + targetWorldPos) / 2f;
            midWorldPos.y += peakHeight;

            Quaternion targetRotation = Quaternion.Euler(0f, -90f, -90f);

            Sequence seq = DOTween.Sequence().SetId(fishItem.transform); // ID for Kill

            seq.Append(fishItem.transform.DOMove(midWorldPos, animationLength / 2f).SetEase(Ease.OutQuad));
            seq.Join(fishItem.transform.DORotate(targetRotation.eulerAngles, animationLength / 2f).SetEase(Ease.InOutSine));

            seq.Append(fishItem.transform.DOMove(targetWorldPos, animationLength / 2f).SetEase(Ease.InQuad));

            pickedUpFish.Push(fishItem);

            seq.OnComplete(() =>
            {
                onComplete?.Invoke();
            });

        }
    }

}