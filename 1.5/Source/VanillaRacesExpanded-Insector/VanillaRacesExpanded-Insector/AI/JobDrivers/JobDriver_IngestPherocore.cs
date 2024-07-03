﻿
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace VanillaRacesExpandedInsector
{
    public class JobDriver_IngestPherocore : JobDriver
    {
        private int useDuration = 100;

      

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            job.count = 1;
            this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
            

            Toil toil = Toils_General.Wait(useDuration);
            toil.WithProgressBarToilDelay(TargetIndex.A);
            toil.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            if (job.targetA.IsValid)
            {
                toil.FailOnDespawnedOrNull(TargetIndex.A);

            }
            yield return toil;
            Toil use = new Toil();

            use.initAction = delegate
            {
                Pawn actor = use.actor;
                

                ThingDef pherocore = job.targetA.Thing.def;

                if(pherocore == InternalDefOf.VFEI2_PherocoreSorne)
                {
                    Dictionary<GeneDef, bool> sorneGenes = GameComponent_UnlockedGenes.Instance.sorne_pherocore_genes;

                    if(sorneGenes.Values.Any(x => x== false))
                    {
                        GeneDef gene = sorneGenes.Keys.Where(x => sorneGenes[x] == false).RandomElement();
                        sorneGenes[gene] = true;
                        GenelineGeneDef genelinegene = gene as GenelineGeneDef;
                        Messages.Message("VRE_PherocoreConsumed".Translate(pherocore.LabelCap,gene.LabelCap,IsEvolutionOrMutation(genelinegene)), pawn, MessageTypeDefOf.PositiveEvent, true);
                        TargetA.Thing.Destroy();
                        Utils.cachedGeneDefsInOrder = null;
                        if (!sorneGenes.Values.Any(x => x == false))
                        {
                            GameComponent_UnlockedGenes.Instance.allSorneGenesUnlocked = true;
                        }

                    } else {
                        Messages.Message("VRE_NoUnlockableGenes".Translate(pherocore.LabelCap), pawn, MessageTypeDefOf.NegativeEvent, true);


                    }

                }
 
                

            };
            use.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return use;
            yield break;

        }

        public static string IsEvolutionOrMutation(GenelineGeneDef gene)
        {
            if (gene.IsMutation) { return "mutation"; } else return "evolution";

        }
    }
}
