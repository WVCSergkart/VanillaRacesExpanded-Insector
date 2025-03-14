﻿using RimWorld;
using Verse;
using Verse.Sound;
using System;
using Verse.AI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace VanillaRacesExpandedInsector
{
    public class HediffComp_ChestburstPregnancy : HediffComp
    {
        public bool miscarriage = true;
        public GeneSet genes;
        public Pawn mother;
        public Pawn father;
       

        public HediffCompProperties_ChestburstPregnancy Props
        {
            get
            {
                return (HediffCompProperties_ChestburstPregnancy)this.props;
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref this.miscarriage, nameof(this.miscarriage), true);
            Scribe_References.Look(ref father, "father", saveDestroyedThings: true);
            Scribe_References.Look(ref mother, "mother", saveDestroyedThings: true);
            Scribe_Deep.Look(ref genes, "genes");
        }

        public override void CompPostMake()
        {
            if (parent.pawn.Faction == Faction.OfPlayer) { StaticCollectionsClass.AddColonistToChestburstImplantationAlert(parent.pawn); }
           

            base.CompPostMake();
        }

        public override void CompPostPostRemoved()
        {
            StaticCollectionsClass.RemoveColonistFromChestburstImplantationAlert(parent.pawn);
            base.CompPostPostRemoved();
            if (miscarriage)
            {
                parent.pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(ThoughtDefOf.Miscarried);
            }

        }

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            base.CompGetGizmos();
            yield return new Command_Target
            {
                defaultLabel = "VRE_InjectChestburstFetus".Translate(),
                defaultDesc = "VRE_InjectChestburstFetusDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Abilities/Ability_InjectChestburster", true),
                targetingParams = new TargetingParameters { canTargetLocations = true },
                action = delegate (LocalTargetInfo target)
                {
                    TryChestburstImplantation(target);
                }
            };



        }

        public void TryChestburstImplantation(LocalTargetInfo target)
        {
            if(target.Pawn != null) {


                if (target.Pawn.IsGhoul)
                {
                    Messages.Message("VRE_CantImplantOnGhoul".Translate(), parent.pawn, MessageTypeDefOf.NegativeEvent, true);
                } else if(target.Pawn.IsShambler)
                {
                    Messages.Message("VRE_CantImplantOnShambler".Translate(), parent.pawn, MessageTypeDefOf.NegativeEvent, true);
                }
                else if (target.Pawn.RaceProps.Humanlike || (target.Pawn.RaceProps.baseBodySize>1 && target.Pawn.DevelopmentalStage == DevelopmentalStage.Adult))
                {
                    Job job = new(InternalDefOf.VRE_ChestburstImplantationJob, target.Pawn);
                    parent.pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                }
                else
                {
                    Messages.Message("VRE_NeedsHumanoidOrAnimal".Translate(), parent.pawn, MessageTypeDefOf.NegativeEvent, true);
                }
            }
        }


    }
}

