# Projet PRBD 2324 - Groupe g01 - Tricount

## Notes de version

### Liste des bugs connus

* pour reset: Rarement lorsqu'on delete un tricount pre existant et qu'on fait reset, le tricount se reouvre automatiquement (cela semble se decider a la compilation)
* pour login/signup: après un signup, si le nouveau user créer un tricount, no a d'ffice un TabDirty malgré un ClearErrors() avant de changer d'onglet


### Liste des fonctionnalités supplémentaires
#### n/a
### Divers

* On a rendu le pseudo unique au signup(pas deux username identiques dans la DB)
* dans l'édition d'un tricount, vu que seul le créateur d'un tricount peut le modifier (vu dans la vidéo) et qu'on ne peut retirer le créateur des participants, le bouton "add myself" ne s'applique que pour un admin
* dans l'édition d'un tricount, seul l'admin peut s'ajouter/se supprimmer des participants d'un tricount
* pour la gestion des opérations: parfois (rarement) editer une operation reouvre les tricount precedemment ouvert
* concernant la qualité du modèle: certain foreach contiennent des requete linq et le code behind est utilisé pour dynamiser la vue